using System;
using System.Threading.Tasks;
using Crumbs.Core.Aggregate;

namespace Crumbs.Core.Snapshot
{
    public class SnapshotNoneStrategy : ISnapshotStrategy
    {
        public Task<bool> ShouldMakeSnapShot(IAggregateRoot aggregate)
        {
            return Task.FromResult(false);
        }

        public bool IsSnapshotable(Type aggregateType)
        {
            return false;
        }
    }
}