using Crumbs.Core.Aggregate;
using System.Threading.Tasks;

namespace Crumbs.Core.Snapshot
{
    public class SnapshotAllStrategy : SnapshotStrategyBase, ISnapshotStrategy
    {
        public Task<bool> ShouldMakeSnapShot(IAggregateRoot aggregate)
        {
            return Task.FromResult(IsSnapshotable(aggregate.GetType()));
        }
    }
}