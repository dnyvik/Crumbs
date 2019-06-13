using System.Reflection;
using Crumbs.Core.Aggregate;

namespace Crumbs.Core.Snapshot
{
    public class SnapshotRestoreUtility : ISnapshotRestoreUtility
    {
        private readonly PropertyInfo _idPropertyInfo = typeof(AggregateRoot).GetProperty(nameof(AggregateRoot.Id));
        private readonly PropertyInfo _versionPropertyInfo = typeof(AggregateRoot).GetProperty(nameof(AggregateRoot.Version));

        public void Restore(IAggregateRoot aggregate, Snapshot snapshot)
        {
            _idPropertyInfo.SetValue(aggregate, snapshot.AggregateId);
            _versionPropertyInfo.SetValue(aggregate, snapshot.Version);

            (aggregate as dynamic).RestoreFromSnapshot((dynamic)snapshot);
        }
    }
}