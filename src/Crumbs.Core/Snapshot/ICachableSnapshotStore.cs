using System;
using System.Threading.Tasks;

namespace Crumbs.Core.Snapshot
{
    public interface ICachableSnapshotStore : ISnapshotStore
    {
        Task WarmupCache();
        Task Invalidate(Guid aggregateId);
    }
}