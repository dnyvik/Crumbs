using System;
using System.Linq;
using System.Threading.Tasks;
using Crumbs.Core.Aggregate;
using Crumbs.Core.Event;
using Crumbs.Core.Exceptions;
using Crumbs.Core.Snapshot;

namespace Crumbs.Core.Repositories
{
    public class AggregateRootRepository : IAggregateRootRepository
    {
        private readonly ISnapshotRestoreUtility _snapshotRestoreUtility;
        private readonly IEventPublisher _eventPublisher;
        private readonly IEventStore _eventStore;
        private readonly ICachableSnapshotStore _cachableSnapshotStore;
        private readonly ISnapshotStore _snapshotStore;
        private readonly ISnapshotStrategy _snapshotStrategy;

        public AggregateRootRepository(
            IEventStore eventStore,
            ISnapshotStore snapshotStore,
            ISnapshotStrategy snapshotStrategy,
            ISnapshotRestoreUtility snapshotRestoreUtility,
            IEventPublisher eventPublisher)
        {
            _snapshotRestoreUtility = snapshotRestoreUtility;
            _eventPublisher = eventPublisher;
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _snapshotStore = snapshotStore;
            _snapshotStrategy = snapshotStrategy ?? throw new ArgumentNullException(nameof(snapshotStrategy));

            // Todo: Handle config flavors for snapshot store (cached vs non cached)

            if (SnapshotStore == null)
            {
                throw new ArgumentException(nameof(ISnapshotStore));
            }
        }

        public async Task Save<T>(T aggregate, Guid userId, Guid? sessionKey = null) where T : IAggregateRoot
        {
            if (!aggregate.HasPendingChanges) return;

            var events = aggregate.FlushUncommitedEvents(sessionKey, userId);
            var sequencedEvents = await _eventStore.Save(events, sessionKey);
            await PublishEvents(sequencedEvents);
            await TryMakeSnapshot(aggregate, sessionKey);
        }

        public async Task<T> Get<T>(Guid aggregateId) where T : class, IAggregateRoot
        {
            return await LoadAggregate<T>(aggregateId);
        }

        public async Task Reload(Guid aggregateId)
        {
            if (_cachableSnapshotStore != null)
            {
                await _cachableSnapshotStore.Invalidate(aggregateId);
            }
        }

        public async Task Delete(Guid aggregateId)
        {
            await SnapshotStore.Delete(aggregateId);
            // Todo: Delete events?
        }

        // Todo: Rename to: MakeSnapshot? ScheduleSnapshot? Maybe this should be lazy?
        public async Task TryMakeSnapshot(IAggregateRoot aggregate, Guid? scope = null)
        {
            if (!await _snapshotStrategy.ShouldMakeSnapShot(aggregate))
            {
                return;
            }

            var snapshot = (aggregate as dynamic).CreateSnapshot();
            snapshot.Version = aggregate.Version;
            snapshot.AggregateId = aggregate.Id;

            await _snapshotStore.Save(snapshot, scope);
        }

        private async Task<T> LoadAggregate<T>(Guid id) where T : class, IAggregateRoot
        {
            var snapshot = await _snapshotStore.Get(id);
            var events = snapshot == null ? await _eventStore.Get(id, -1) : null;

            if (snapshot == null && !events.Any())
            {
                throw new AggregateNotFoundException(typeof(T), id);
            }

            var isSnapshotable = _snapshotStrategy.IsSnapshotable(typeof(T));

            var aggregate = isSnapshotable || snapshot == null
                ? AggregateFactory.Instance.CreateAggregate<T>()
                : AggregateFactory.Instance.CreateAggregate<T>(snapshot);

            if (snapshot != null)
            {
                _snapshotRestoreUtility.Restore(aggregate, snapshot);
            }

            if (events?.Any() ?? false)
            {
                aggregate.LoadFromHistory(events);
            }

            return aggregate;
        }

        private async Task PublishEvents(System.Collections.Generic.IEnumerable<IDomainEvent> sequencedEvents)
        {
            foreach (var domainEvent in sequencedEvents)
            {
                await _eventPublisher.Publish(domainEvent);
            }
        }

        private ISnapshotStore SnapshotStore => _cachableSnapshotStore ?? _snapshotStore;
    }
}