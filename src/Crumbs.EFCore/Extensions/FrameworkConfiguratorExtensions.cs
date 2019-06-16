using Crumbs.Core.Configuration;
using Crumbs.Core.Event;
using Crumbs.Core.Session;
using Crumbs.Core.Snapshot;
using Crumbs.EFCore.Session;

namespace Crumbs.EFCore.Extensions
{
    public static partial class FrameworkConfiguratorExtensions
    {
        public static FrameworkConfigurator UseSqlite(this FrameworkConfigurator configurator,
            string connectionString)
        {
            return UseProvider(configurator, connectionString, ProviderType.Sqlite);
        }

        public static FrameworkConfigurator UseMySql(this FrameworkConfigurator configurator,
            string connectionString)
        {
            return UseProvider(configurator, connectionString, ProviderType.MySql);
        }

        public static FrameworkConfigurator UseDefaultStores(this FrameworkConfigurator configurator)
        {
            configurator.RegisterTransientType<IEventStore, EventStore>();
            configurator.RegisterTransientType<ISessionStore, SessionStore>();
            configurator.RegisterTransientType<ISnapshotStore, SnapshotStore>(); // Todo: Select flavor

            configurator.RegisterSingelton<IDataStoreConnectionFactory, DataStoreConnectionFactory>();
            configurator.RegisterSingelton<IFrameworkContextFactory, DataStoreConnectionFactory>();
            // Todo: EventHandler state store

            return configurator;
        }

        private static FrameworkConfigurator UseProvider(this FrameworkConfigurator configurator, string connectionString, ProviderType providerType)
        {
            configurator.AddConfigurationValue(DataStoreConnectionFactory.EFCoreConnectionStringKey, connectionString);
            configurator.AddConfigurationValue(DataStoreConnectionFactory.ProviderTypeKey, providerType);
            return RegisterMigrationAction(configurator);
        }

        private static FrameworkConfigurator RegisterMigrationAction(this FrameworkConfigurator configurator)
        {
            // Todo: Rewrite to public async Task<T> InitilizeAction<T>(Func<T, Task> action)
            return configurator.RegisterInitializationAction(resolver =>
            {
                var factory = resolver.Resolve<IFrameworkContextFactory>();
                // Todo: Rewrite (need to make sure migration is done before run)
                var context = factory.CreateContext().Result;
                context.Migrate().Wait();
            });
        }
    }
}
