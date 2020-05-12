using Crumbs.Core.Event;
using Crumbs.Core.Event.EventualConsistency;
using Crumbs.Core.Event.Framework;
using Crumbs.EventualConsistency.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Crumbs.EventualConsistency
{
    public abstract class StatefulEventuallyConsistentEventHandler<T> : IStatefulEventHandler<T>, IDisposable where T : IEventHandlerState
    {
        private readonly IEventStreamer _eventStreamer;
        private readonly IEventHandlerStateStore _eventHandlerStateStore;
        private readonly IEventStore _eventStore;

        private IDisposable _eventStreamSubscription;
        private IStateContainer<T> _stateContainer;

        private readonly Dictionary<Type, Action<IDomainEvent>> _handlerMap
            = new Dictionary<Type, Action<IDomainEvent>>();

        // How many events should we get from event store at a time?
        private const int HistoricalEventBatchSize = 2000;

        // If running off live stream
        protected bool IsLoadingHistoricalEvents;
        private readonly Dictionary<Guid, List<IDomainEvent>> _sessionBuffer = new Dictionary<Guid, List<IDomainEvent>>();
        private readonly Type _sessionCommittedEventType = typeof(SessionCommittedEvent);
        private readonly Type _sessionRolledBackEventType = typeof(SessionRolledBackEvent);

        // If run off historical polling
        private bool _pollerIsRunning;
        private readonly TimeSpan _historicalPollingInterval = TimeSpan.FromSeconds(20); // Todo: Config

        protected StatefulEventuallyConsistentEventHandler(IEventStore eventStore,
                                               IEventStreamer eventStreamer,
                                               IEventHandlerStateStore eventHandlerStateStore)
        {
            _eventStore = eventStore;
            _eventStreamer = eventStreamer;
            _eventHandlerStateStore = eventHandlerStateStore;
            RegisterHandlers();
        }

        private void RegisterHandlers()
        {
            var eventTypes = GetType()
                .GetInterfaces()
                .Where(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition()
                            == typeof(IHistoricalEventHandler<>))
                .Select(i => i.GetGenericArguments()[0]);

            foreach (var eventType in eventTypes)
            {
                _handlerMap.Add(eventType, e => (this as dynamic).Handle((dynamic)e));
            }
        }

        public async Task Initialize(Guid stateKey, T initalState = default, bool useLiveEventStream = true)
        {
            await InitializeState(stateKey, initalState);

            // First time initialization
            if (initalState != null)
            {
                SaveState();
            }

            IsLoadingHistoricalEvents = true;

            if (useLiveEventStream)
            {
                SubscriptionToLiveEvent();
            }
            else
            {
                SubscribeToHistoricalStream();
            }
        }

        private void SubscriptionToLiveEvent()
        {
            _eventStreamSubscription = CreateHistoricalEventStream()
                .HotConcat(_eventStreamer.EventStream)
                .SubscribeOn(NewThreadScheduler.Default) //We do not want to block controller calls on initialize while we are spooling up events.
                .Select(e => Observable.FromAsync(async () => await HandleNextEvent(e)))
                .Subscribe();
        }

        private void SubscribeToHistoricalStream()
        {
            _pollerIsRunning = true;
            _eventStreamSubscription = CreatePollingHistoricalEventStream()
                .SubscribeOn(NewThreadScheduler.Default) //We do not want to block controller calls on initialize while we are spooling up events.
                .Select(e => Observable.FromAsync(async () => await HandleEvent(e)))
                .Subscribe();
        }

        private async Task InitializeState(Guid stateKey, T initalState)
        {
            _stateContainer = await _eventHandlerStateStore.Get<T>(stateKey) ?? CreateDefaultStateContainer(stateKey, initalState);

            if (_stateContainer.HandlerStatus == StatefulHandlerStatus.Faulted)
            {
                throw new Exception("Prior state suggests that handler is in an invalid state. Consider reinitializing handler.");
            }

            _stateContainer.HandlerStatus = StatefulHandlerStatus.SpoolingHistory;
            State = _stateContainer.State;
        }

        private StateContainer<T> CreateDefaultStateContainer(Guid stateKey, T initalState)
        {
            return new StateContainer<T>
            {
                Id = stateKey,
                ProcessedEventId = -1,
                State = initalState,
                HandlerStatus = StatefulHandlerStatus.SpoolingHistory,
            };
        }

        private async Task HandleNextEvent(IDomainEvent domainEvent)
        {
            if (IsLoadingHistoricalEvents)
            {
                await HandleEvent(domainEvent);
                return;
            }

            var eventType = domainEvent.GetType();

            // ReSharper disable once PossibleInvalidOperationException (Should always be set by framework before it is pushed on bus)
            var sessionKey = domainEvent.SessionKey.Value;

            if (eventType == _sessionCommittedEventType)
            {
                await FlushSession(sessionKey);
                return;
            }
            if (eventType == _sessionRolledBackEventType)
            {
                RollBackSession(sessionKey);
                return;
            }

            AddToSessionBuffer(domainEvent, sessionKey);
        }

        private async Task HandleEvent(IDomainEvent e)
        {
            try
            {
                if (_stateContainer.ProcessedEventId >= e.Id)
                {
                    throw new Exception("Out of sequence events");
                }

                var eventType = e.GetType();

                if (_handlerMap.ContainsKey(eventType))
                {
                    _handlerMap[eventType](e);
                }

                UpdateAuditInformation(e);
            }
            catch (Exception exception)
            {
                //Todo: Set fault message in DB (from exception)
                await SetFaultedState();
                await RollbackState();
            }
        }

        private void UpdateAuditInformation(IDomainEvent e)
        {
            _stateContainer.LastUpdated = e.Timestamp;
            _stateContainer.ProcessedEventId = e.Id;
        }

        private void AddToSessionBuffer(IDomainEvent domainEvent, Guid sessionKey)
        {
            if (_sessionBuffer.ContainsKey(sessionKey))
            {
                _sessionBuffer[sessionKey].Add(domainEvent);
            }
            else
            {
                _sessionBuffer[sessionKey] = new List<IDomainEvent> { domainEvent };
            }
        }

        private async Task FlushSession(Guid sessionKey)
        {
            IList<IDomainEvent> events = null;

            if (_sessionBuffer.ContainsKey(sessionKey))
            {
                events = _sessionBuffer[sessionKey];
                _sessionBuffer.Remove(sessionKey);
            }

            if (events != null)
            {
                // Session might be committed before history is loaded, so we might get the same event from event store and the live stream
                var nonProcessedEvents = events.Where(e => e.Id > _stateContainer.ProcessedEventId);
                foreach (var domainEvent in nonProcessedEvents)
                {
                    await HandleEvent(domainEvent);
                }
            }

            await Save();
        }

        private void RollBackSession(Guid sessionKey)
        {
            _sessionBuffer.Remove(sessionKey);
        }

        private IObservable<IDomainEvent> CreateHistoricalEventStream()
        {
            return Observable.Create<IDomainEvent>(async observer =>
            {
                var eventCount = HistoricalEventBatchSize;

                while (eventCount == HistoricalEventBatchSize)
                {
                    var domainEvents = await _eventStore
                        .GetAllAfter(_stateContainer.ProcessedEventId, HistoricalEventBatchSize);

                    foreach (var e in domainEvents)
                    {
                        observer.OnNext(e);
                    }

                    eventCount = domainEvents.Count;
                }

                observer.OnCompleted();
                await HistoryLoaded();
                return Disposable.Empty;
            });
        }

        private IObservable<IDomainEvent> CreatePollingHistoricalEventStream()
        {
            return Observable.Create<IDomainEvent>(async observer =>
            {
                while (_pollerIsRunning)
                {
                    var eventCount = HistoricalEventBatchSize;

                    while (eventCount == HistoricalEventBatchSize)
                    {
                        var domainEvents = await _eventStore
                            .GetAllAfter(_stateContainer.ProcessedEventId, HistoricalEventBatchSize);

                        foreach (var e in domainEvents)
                        {
                            observer.OnNext(e);
                        }

                        eventCount = domainEvents.Count;
                    }

                    await Save();

                    await Task.Delay(_historicalPollingInterval);
                }

                observer.OnCompleted();

                return Disposable.Empty;
            });
        }

        public abstract T State { get; set; }

        public async Task Delete()
        {
            UnsubscribeToEventStream();
            OnDeleteData(_stateContainer.Id);
            await _eventHandlerStateStore.Delete(_stateContainer.Id);
        }

        public bool IsFaulted { get; private set; }

        private async Task HistoryLoaded()
        {
            IsLoadingHistoricalEvents = false;
            _stateContainer.HandlerStatus = StatefulHandlerStatus.Running;
            await Save();
        }

        private async Task Save()
        {
            try
            {
                OnSaveData(_stateContainer.Id);
                SaveState();
            }
            catch (Exception)
            {
                await SetFaultedState();
            }
        }

        private async Task SetFaultedState()
        {
            IsFaulted = true;
            UnsubscribeToEventStream();

            try
            {
                await RollbackState();
                _stateContainer.HandlerStatus = StatefulHandlerStatus.Faulted;
                await _eventHandlerStateStore.Save(_stateContainer);
                OnFaulted(_stateContainer.Id);
            }
            catch (Exception) { }
        }

        protected abstract void OnSaveData(Guid stateKey);
        protected abstract void OnDeleteData(Guid stateKey);
        protected abstract void OnFaulted(Guid stateKey);

        private void SaveState()
        {
            _eventHandlerStateStore.Save(_stateContainer);
        }

        private async Task RollbackState()
        {
            await InitializeState(_stateContainer.Id, default);
        }

        public void Dispose()
        {
            UnsubscribeToEventStream();
        }

        private void UnsubscribeToEventStream()
        {
            _pollerIsRunning = false;
            _eventStreamSubscription?.Dispose();

            _eventStreamSubscription = null;
        }
    }
}