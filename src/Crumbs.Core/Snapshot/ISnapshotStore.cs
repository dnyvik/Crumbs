using System;
using System.Threading.Tasks;

namespace Crumbs.Core.Snapshot
{
    // Todo: Async postfix
    public interface ISnapshotStore
    {
        Task<Snapshot> Get(Guid aggregateId);
        Task<int?> GetVersion(Guid aggregateId);
        Task Save(Snapshot snapshot, Guid? sessionKey);
        Task Delete(Guid aggregateId);
        Task DeleteAll();

        // Todo: Rebuild snapshots? Get all aggregate Ids or get all snapshot ids and rebuild.
    }
}