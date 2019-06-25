using Crumbs.Core.Snapshot;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Crumbs.EFCore.Session
{
    public class SnapshotStore : ISnapshotStore
    {
        private readonly ISnapshotSerializer _snapshotSerializer;
        private readonly IFrameworkContextFactory _frameworkContextFactor;

        public SnapshotStore(
            ISnapshotSerializer snapshotSerializer,
            IFrameworkContextFactory frameworkContextFactor)
        {
            _snapshotSerializer = snapshotSerializer;
            _frameworkContextFactor = frameworkContextFactor;
        }

        public async Task Delete(Guid aggregateId)
        {
            using (var context = await _frameworkContextFactor.CreateContext())
            {
                var snapshot = context.Snapshots.SingleOrDefault(s => s.AggregateId == aggregateId);

                if (snapshot != null)
                {
                    context.Snapshots.Remove(snapshot);
                    await context.SaveChangesAsync(); //Todo: CT
                }
            }
        }

        // Todo: Should be optimized for provider. Like "TRUNCATE TABLE [Snapshot]" for MSSQL..
        public Task DeleteAll()
        {
            throw new NotImplementedException();
        }

        // Todo: Compiled query
        public async Task<Snapshot> Get(Guid aggregateId)
        {
            using (var context = await _frameworkContextFactor.CreateContext())
            {
                var snapshot = await context.Snapshots.AsNoTracking()
                    .SingleOrDefaultAsync(s => s.AggregateId == aggregateId);

                return snapshot != null ? Deserialize(snapshot) : null;
            }
        }

        // Todo: Compiled query
        public async Task<int?> GetVersion(Guid aggregateId)
        {
            using (var context = await _frameworkContextFactor.CreateContext())
            {
                return await context.Snapshots
                    .Where(s => s.AggregateId == aggregateId)
                    .Select(s => s == null ? (int?)null : s.Version)
                    .SingleOrDefaultAsync();
            }
        }

        // Todo: Do this lazy and out of session? Possible future performance optimization.
        public async Task Save(Snapshot snapshot, Guid? sessionKey)
        {
            using (var context = await _frameworkContextFactor.CreateContext(sessionKey))
            {
                var existingSnapshot = context.Snapshots.SingleOrDefault(s => s.AggregateId == snapshot.AggregateId);

                if (existingSnapshot != null)
                {
                    existingSnapshot.Version = snapshot.Version;
                    existingSnapshot.Type = snapshot.GetType().AssemblyQualifiedName;
                    existingSnapshot.Content = _snapshotSerializer.Serialize(snapshot);
                }
                else
                {
                    context.Snapshots.Add(new Models.Snapshot
                    {
                        AggregateId = snapshot.AggregateId,
                        Version = snapshot.Version,
                        Type = snapshot.GetType().AssemblyQualifiedName,
                        Content = _snapshotSerializer.Serialize(snapshot),
                    });
                }

                await context.SaveChangesAsync(); // Todo: CT
            }
        }

        private Snapshot Deserialize(Models.Snapshot dto)
        {
            // Todo: Type cache?
            var snapshot = _snapshotSerializer.Deserialize(dto.Content, Type.GetType(dto.Type));
            snapshot.AggregateId = dto.AggregateId;
            snapshot.Version = dto.Version;

            return snapshot;
        }
    }
}
