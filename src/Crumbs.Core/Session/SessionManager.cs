using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crumbs.Core.Aggregate;
using Crumbs.Core.Command;
using Crumbs.Core.Configuration;
using Crumbs.Core.Configuration.SessionProfiles;
using Crumbs.Core.Event;
using Crumbs.Core.Event.Framework;
using Crumbs.Core.Exceptions;
using Crumbs.Core.Extensions;
using Crumbs.Core.Repositories;

namespace Crumbs.Core.Session
{
    public class SessionManager : ISessionManager
    {
        private readonly IEventPublisher _eventPublisher;
        private IDataStoreConnectionFactory _dataStoreConnectionFactory;

        // Todo: Config params
        private const IsolationLevel TransactionIsolationLevel = IsolationLevel.ReadCommitted;

        private readonly Dictionary<Guid, AggregateDescriptor> _trackedAggregates;
        private readonly Dictionary<Guid, IDataStoreScope> _activeDatabaseScopes;
        private static readonly ThreadLocal<Random> Random = new ThreadLocal<Random>(() => new Random(), false);

        private readonly AsyncLock _aggregateTrackerMutex;
        private readonly AsyncLock _databaseScopeMutex;
        private readonly ISessionProfile _sessionProfile;
        private IAggregateRootRepository _repository;
        private ISessionTracker _sessionTracker;
        private bool _initialized;

        public SessionManager(
            IEventPublisher eventPublisher,
            ISessionProfile sessionProfile) // Todo: Scope these to sessions instead? Could be a usefull overload.
        {
            _eventPublisher = eventPublisher;
            _trackedAggregates = new Dictionary<Guid, AggregateDescriptor>();
            _activeDatabaseScopes = new Dictionary<Guid, IDataStoreScope>();
            _aggregateTrackerMutex = new AsyncLock();
            _databaseScopeMutex = new AsyncLock();
            _sessionProfile = sessionProfile;
        }

        public async Task<ISession> CreateSession(Guid sessionKey)
        {
            if (!_initialized) throw new InvalidOperationException($"Please call {nameof(Initialize)} before creating session.");

            if (await _sessionTracker.HasCompleted(sessionKey)) throw new SessionAlreadyCommittedException(sessionKey);

            return new Session(this, sessionKey);
        }

        public async Task<T> LoadAggregate<T>(Guid id, CancellationToken ct) where T : class, IAggregateRoot
        {
            for (var loadAttempts = 0; ; loadAttempts++)
            {
                using (var asyncLock = await _aggregateTrackerMutex.LockAsync(_sessionProfile.LoadAttemptTimeout, ct))
                {
                    var lockAcquired = asyncLock != null;

                    if (lockAcquired && !_trackedAggregates.ContainsKey(id))
                    {
                        var aggregate = await _repository.Get<T>(id);

                        _trackedAggregates.Add(aggregate.Id, new AggregateDescriptor
                        {
                            Aggregate = aggregate,
                            Version = aggregate.Version
                        });

                        return aggregate;
                    }
                }

                ct.ThrowIfCancellationRequested();

                if (loadAttempts > _sessionProfile.MaxLoadAttempts)
                {
                    break;
                }

                await Task.Delay(Backoff);
            }

            throw new MaxRetryLimitExceededException(_sessionProfile.MaxLoadAttempts, id, typeof(T));
        }

        private TimeSpan Backoff
        {
            get
            {
                if (_sessionProfile.MinBackoffBetweenLoadAttempts < _sessionProfile.MaxBackoffBetweenLoadAttempts)
                {
                    var min = _sessionProfile.MinBackoffBetweenLoadAttempts.Milliseconds;
                    var max = _sessionProfile.MaxBackoffBetweenLoadAttempts.Milliseconds;
                    var backoff = Random.Value.Next(min, max);

                    return TimeSpan.FromMilliseconds(backoff);
                }

                return TimeSpan.Zero;
            }
        }

        public async Task Commit(ISession session, ICommand command)
        {
            using (var connection = await _dataStoreConnectionFactory.Connect())
            {
                using (var transaction =  connection.BeginTransaction(TransactionIsolationLevel))
                {
                    try
                    {
                        using (await _databaseScopeMutex.LockAsync())
                        {
                            _activeDatabaseScopes[session.Key] = new DatabaseScope(connection, transaction);
                        }

                        foreach (var descriptor in session.TrackedAggregates)
                        {
                            if (descriptor.IsMarkedForDeletion)
                            {
                                await _repository.Delete(descriptor.Aggregate.Id);
                                await _eventPublisher.Publish(new AggregateDeletedEvent(descriptor.Aggregate.Id, command.Id, command.UserId));
                            }
                            else
                            {
                                await _repository.Save(descriptor.Aggregate, command.UserId, session.Key);
                            }
                        }

                        await _sessionTracker.CommitSession(command);

                        transaction.Commit();
                    }
                    catch (Exception e) // Todo: For testing - remove later
                    {
                        transaction.Rollback();
                        await _sessionTracker.RollbackSession(session.Key);
                        await _eventPublisher.Publish(new SessionRolledBackEvent(command.Id, command.UserId));
                        throw;
                    }
                    finally
                    {
                        await Cleanup(session);
                    }
                }
            }

            await _eventPublisher.Publish(new SessionCommittedEvent(command.Id, command.UserId));
        }

        private async Task Cleanup(ISession session, bool rollback = false)
        {
            await RemoveDatabaseScope(session);

            if (rollback)
            {
                await RollbackAggregates(session);
            }

            await Untrack(session.TrackedAggregates);
        }

        private async Task RemoveDatabaseScope(ISession session)
        {
            using (await _databaseScopeMutex.LockAsync())
            {
                _activeDatabaseScopes.Remove(session.Key);
            }
        }

        private async Task RollbackAggregates(ISession session)
        {
            foreach (var aggregateDescriptor in session.TrackedAggregates.Where(d => d.Aggregate.HasPendingChanges))
            {
                await _repository.Reload(aggregateDescriptor.Aggregate.Id);
            }
        }

        public async Task<IDataStoreScope> GetScopeForSession(Guid sessionKey, CancellationToken ct = default)
        {
            IDataStoreScope scope;
            using (await _databaseScopeMutex.LockAsync(ct))
            {
                _activeDatabaseScopes.TryGetValue(sessionKey, out scope);
            }

            if (scope == null)
            {
                throw new MissingDataStoreScopeException(sessionKey);
            }

            return scope;
        }

        /// <summary>
        /// Bad design, circular dependency :(
        /// </summary>
        /// <param name="dataStoreConnectionFactory"></param>
        public void Initialize(
            IDataStoreConnectionFactory dataStoreConnectionFactory,
            IAggregateRootRepository repository,
            ISessionTracker sessionTracker)
        {
            _dataStoreConnectionFactory = dataStoreConnectionFactory;
            _repository = repository;
            _sessionTracker = sessionTracker;

            // Todo: Why did I load all prior sessions into memory here?
            //sessionTracker.Initialize();

            _initialized = true;
        }

        // Todo: Add config profiles for backoffs and retries (i.e. Patient, FailFast.. and so on)

        public async Task Rollback(ISession session)
        {
            try
            {
                var scope = await GetScopeForSession(session.Key);
                scope?.Transaction?.Rollback();
            }
            finally
            {
                await Cleanup(session, true);
            }
        }

        private async Task Untrack(IEnumerable<AggregateDescriptor> aggregateDescriptors)
        {
            using (await _aggregateTrackerMutex.LockAsync())
            {
                foreach (var descriptor in aggregateDescriptors)
                {
                    _trackedAggregates.Remove(descriptor.Aggregate.Id);
                }
            }
        }
    }
}