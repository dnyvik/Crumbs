using System;
using System.Threading.Tasks;
using Crumbs.Core.Aggregate;

namespace Crumbs.Core.Snapshot
{
    public interface ISnapshotStrategy
    {
        Task<bool> ShouldMakeSnapShot(IAggregateRoot aggregate);
        bool IsSnapshotable(Type aggregateType);
    }
}