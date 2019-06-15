using Crumbs.Core.Configuration;
using Crumbs.Core.Event;
using Crumbs.Core.Session;
using Crumbs.Core.Snapshot;
using Crumbs.EFCore.ProviderContexts;
using Crumbs.EFCore.Session;
using Microsoft.Extensions.DependencyInjection;

namespace Crumbs.EFCore.Extensions
{
    public static partial class FrameworkConfiguratorExtensions
    {
        public static FrameworkConfigurator UseSqlite(this FrameworkConfigurator configurator, string connectionString)
        {
            configurator.AddConfigurationValue(DataStoreConnectionFactory.SqliteConnectionStringKey, connectionString);
            configurator.AddConfigurationValue(DataStoreConnectionFactory.ProviderTypeKey, ProviderType.Sqlite);

            return configurator;
        }

        // Todo: Expand to use scoped dependency? IServiceScope for session key?
        // Need to expose transaction to make it easier to explicitly fail it? Or use implicit fail? Which one is the pit of success?
        // Todo: Delete me after scoping concept is landed
        public static FrameworkConfigurator UseSqliteTest(this FrameworkConfigurator configurator,
            IServiceCollection serviceCollection, string connectionString)
        {
            configurator.AddConfigurationValue(DataStoreConnectionFactory.SqliteConnectionStringKey, connectionString);
            configurator.AddConfigurationValue(DataStoreConnectionFactory.ProviderTypeKey, ProviderType.Sqlite);

            // Todo: Check if ioc is wrapper and cast? Else just add singleton?            
            // Todo: pooled + session scope?
            // Todo: Do wee need to expose this to the rest of the application?
            serviceCollection.AddDbContext<IFrameworkContext, SqliteDbContext>((options) =>
            {
            }, contextLifetime: ServiceLifetime.Transient);

            return configurator;
        }

        public static FrameworkConfigurator UseMySqlTest(this FrameworkConfigurator configurator,
            IServiceCollection serviceCollection, string connectionString)
        {
            configurator.AddConfigurationValue(DataStoreConnectionFactory.MySqlConnectionStringKey, connectionString);
            configurator.AddConfigurationValue(DataStoreConnectionFactory.ProviderTypeKey, ProviderType.MySql);

            // Todo: Check if ioc is wrapper and cast? Else just add singleton?

            // Todo: pooled + session scope?
            // Todo: Do wee need to expose this to the rest of the application?
            //serviceCollection.AddDbContext<IFrameworkContext, MySqlDbContext>((options) =>
            //{
            //}, contextLifetime: ServiceLifetime.Transient);

            serviceCollection.AddTransient<IFrameworkContext, MySqlDbContext>();

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
        public static FrameworkConfigurator UseServiceCollection(this FrameworkConfigurator configurator, IServiceCollection serviceCollection)
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


            // Todo: Where should we put this?
            configurator.RegisterInitializationAction(async resolver =>
            {
                var factory = resolver.Resolve<IFrameworkContextFactory>();
                var context = await factory.CreateContext();
                await context.Migrate();
            });

            return configurator;
        }
    }

}
