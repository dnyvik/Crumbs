using Crumbs.Core.Configuration;
using Crumbs.Core.Event;
using Crumbs.Core.Session;
using Crumbs.Core.Snapshot;
using Crumbs.EFCore.Session;
using Microsoft.Extensions.DependencyInjection;

namespace Crumbs.EFCore.Extensions
{
    public static partial class FrameworkConfiguratorExtensions
    {
        public static FrameworkConfigurator UseSqlite(this FrameworkConfigurator configurator,
            string connectionString)
        {
            configurator.AddConfigurationValue(DataStoreConnectionFactory.SqliteConnectionStringKey, connectionString);
            configurator.AddConfigurationValue(DataStoreConnectionFactory.ProviderTypeKey, ProviderType.Sqlite);

            return configurator;
        }

        public static FrameworkConfigurator UseMySql(this FrameworkConfigurator configurator,
            string connectionString)
        {
            configurator.AddConfigurationValue(DataStoreConnectionFactory.MySqlConnectionStringKey, connectionString);
            configurator.AddConfigurationValue(DataStoreConnectionFactory.ProviderTypeKey, ProviderType.MySql);

            return configurator;
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

        // Todo: Seperate project?
        public static FrameworkConfigurator UseServiceCollection(this FrameworkConfigurator configurator,
            IServiceCollection serviceCollection)
        {
            var wrapper = new ServiceCollectionWrapper(serviceCollection);

            configurator.SetDependencyFramework(wrapper);

            // Todo: Rewrite to public async Task<T> InitilizeAction<T>(Func<T, Task> action)
            configurator.RegisterInitializationAction(resolver =>
            {
                var factory = resolver.Resolve<IFrameworkContextFactory>();
                // Todo: Rewrite (need to make sure migration is done before run)
                var context = factory.CreateContext().Result;
                context.Migrate().Wait();
            });

            return configurator;
        }
    }
}
