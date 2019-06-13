using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Crumbs.Core.Aggregate;
using Crumbs.Core.Command;
using Crumbs.Core.Exceptions;

namespace Crumbs.Core.Session
{
    public class Session : ISession
    {
        private readonly ISessionManager _sessionManager;
        private readonly Dictionary<Guid, AggregateDescriptor> _trackedAggregates;
        private bool _committed;

        public Session(ISessionManager sessionManager, Guid sessionKey)
        {
            _sessionManager = sessionManager;
            _trackedAggregates = new Dictionary<Guid, AggregateDescriptor>();
            Key = sessionKey;
        }

        public void Add<T>(T aggregate) where T : IAggregateRoot
        {
            Track(aggregate);
        }

        private void Track<T>(T aggregate, bool markForDeletion = false) where T : IAggregateRoot
        {
            if (!IsTracked(aggregate.Id))
            {
                _trackedAggregates.Add(aggregate.Id,
                                       new AggregateDescriptor
                                       {
                                           Aggregate = aggregate,
                                           Version = aggregate.Version,
                                           IsMarkedForDeletion = markForDeletion
                                       });
            }
            else if (_trackedAggregates[aggregate.Id].Aggregate != (IAggregateRoot)aggregate)
            {
                throw new ConcurrencyException(aggregate.Id);
            }
        }

        public async Task<T> Load<T>(Guid aggregateId, CancellationToken ct = default) where T : class, IAggregateRoot
        {
            return await Load<T>(aggregateId, false, ct);
        }

        private async Task<T> Load<T>(Guid aggregateId, bool markForDeletion, CancellationToken ct) where T : class, IAggregateRoot
        {
            if (IsTracked(aggregateId))
            {
                return (T)_trackedAggregates[aggregateId].Aggregate;
            }

            var aggregate = await _sessionManager.LoadAggregate<T>(aggregateId, ct);

            Track(aggregate, markForDeletion);

            return aggregate;
        }

        public async Task<IReadOnlyList<T>> Load<T>(IEnumerable<Guid> aggregateIds, CancellationToken ct = default) where T : class, IAggregateRoot
        {
            var aggregates = new List<T>();

            foreach (var aggregateId in aggregateIds)
            {
                aggregates.Add(await Load<T>(aggregateId, ct));
            }

            return aggregates.AsReadOnly();
        }

        public async Task Delete<T>(Guid aggregateId, CancellationToken ct = default) where T : class, IAggregateRoot
        {
            await Load<T>(aggregateId, true, ct);
        }

        private bool IsTracked(Guid id)
        {
            return _trackedAggregates.ContainsKey(id);
        }

        public async Task Commit(ICommand command)
        {
            if (_committed) throw new InvalidOperationException("Session has already committed.");

            try
            {
                await _sessionManager.Commit(this, command);
            }
            finally
            {
                _committed = true;
            }
        }

        public IEnumerable<AggregateDescriptor> TrackedAggregates => _trackedAggregates.Values;
        public Guid Key { get; }

        public void Dispose()
        {
            if (!_committed) _sessionManager.Rollback(this);
        }
    }
}