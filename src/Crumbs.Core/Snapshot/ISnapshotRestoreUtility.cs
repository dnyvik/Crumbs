using Crumbs.Core.Aggregate;

namespace Crumbs.Core.Snapshot
{
    public interface ISnapshotRestoreUtility
    {
        void Restore(IAggregateRoot aggregate, Snapshot snapshot);
    }
}