using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Crumbs.Core.Aggregate;
using Crumbs.Core.Command;

namespace Crumbs.Core.Session
{
    public interface ISession : IDisposable
    {
        void Add<T>(T aggregate) where T : IAggregateRoot;
        Task<T> Load<T>(Guid aggregateId, CancellationToken ct = default) where T : class, IAggregateRoot;
        Task<IReadOnlyList<T>> Load<T>(IEnumerable<Guid> aggregateIds, CancellationToken ct = default) where T : class, IAggregateRoot;
        Task Delete<T>(Guid aggregateId, CancellationToken ct = default) where T : class, IAggregateRoot;
        Task Commit(ICommand userId);
        IEnumerable<AggregateDescriptor> TrackedAggregates { get; }
        Guid Key { get; }
    }
}