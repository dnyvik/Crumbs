using Crumbs.Core.Aggregate;
using Crumbs.Core.Configuration;
using Crumbs.Core.Extensions;
using System.Threading.Tasks;

namespace Crumbs.Core.Snapshot
{
    public class SnapshotOnEventCountStrategy : SnapshotStrategyBase, ISnapshotStrategy
    {
        public const string ConfigurationKey = nameof(SnapshotOnEventCountStrategy);

        private readonly int _eventsBeforeSnapshot;
        private readonly ISnapshotStore _snapshotStore;

        public SnapshotOnEventCountStrategy(IFrameworkConfiguration frameworkConfiguration, ISnapshotStore snapshotStore)
        {
            _eventsBeforeSnapshot = frameworkConfiguration.GetValue<int>(ConfigurationKey);
            _snapshotStore = snapshotStore;
        }

        public async Task<bool> ShouldMakeSnapShot(IAggregateRoot aggregate)
        {
            var lastSnapshotVersion = await _snapshotStore.GetVersion(aggregate.Id);

            if (!lastSnapshotVersion.HasValue)
            {
                return true;
            }

            var newEvents = aggregate.Version - lastSnapshotVersion;

            return newEvents >= _eventsBeforeSnapshot;
        }
    }
}
