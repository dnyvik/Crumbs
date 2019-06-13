using Crumbs.Core.Configuration;
using Crumbs.Core.Snapshot;
using System;

namespace Crumbs.Core.Extensions
{
    public static class FrameworkConfiguratorExtensions
    {
        // Snapshot strategy
        public static FrameworkConfigurator UseSnapshotNoneStrategy(this FrameworkConfigurator configurator)
        {
            configurator.RegisterSingelton<ISnapshotStrategy, SnapshotNoneStrategy>();
            return configurator;
        }

        public static FrameworkConfigurator UseSnapshotAllStrategy(this FrameworkConfigurator configurator)
        {
            configurator.RegisterSingelton<ISnapshotStrategy, SnapshotAllStrategy>();
            return configurator;
        }

        public static FrameworkConfigurator UseSnapshotOnEventCountStrategy(this FrameworkConfigurator configurator, int eventCount = 50)
        {
            configurator.AddConfigurationValue(SnapshotOnEventCountStrategy.ConfigurationKey, eventCount);
            configurator.RegisterSingelton<ISnapshotStrategy, SnapshotOnEventCountStrategy>();
            return configurator;
        }

        // Locking
        public static FrameworkConfigurator SetLockingStrategy(this FrameworkConfigurator configurator, LockingStrategy lockingStrategy)
        {
            throw new NotSupportedException("Not yet supported.");
            return configurator;
        }
    }
}

