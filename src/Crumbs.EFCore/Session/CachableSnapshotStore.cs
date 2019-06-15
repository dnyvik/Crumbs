using Crumbs.Core.Snapshot;
using System;
using System.Threading.Tasks;

namespace Crumbs.EFCore.Session
{
    public class CachableSnapshotStore : ICachableSnapshotStore
    {
        private readonly ISnapshotStore _snapshotStore;

        // Todo: Use snapshot store for base calls (when snapshot isn't in cache)

        public CachableSnapshotStore(
            ISnapshotSerializer snapshotSerializer,
            IFrameworkContextFactory frameworkContextFactor,
            ISnapshotStore snapshotStore)
        {
            _snapshotStore = snapshotStore;
        }

        public Task WarmupCache()
        {
            throw new NotImplementedException();
        }

        public Task Invalidate(Guid aggregateId)
        {
            throw new NotImplementedException();
        }

        public Task<Snapshot> Get(Guid aggregateId)
        {
            throw new NotImplementedException();
        }

        public Task<int?> GetVersion(Guid aggregateId)
        {
            throw new NotImplementedException();
        }

        public Task Save(Snapshot snapshot, Guid? sessionKey)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Guid aggregateId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAll()
        {
            throw new NotImplementedException();
        }
    }
}
