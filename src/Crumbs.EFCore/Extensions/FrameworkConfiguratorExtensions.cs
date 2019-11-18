using Crumbs.Core.Configuration;
using Crumbs.Core.Event;
using Crumbs.Core.Event.EventualConsistency;
using Crumbs.Core.Session;
using Crumbs.Core.Snapshot;
using Crumbs.EFCore.Event;
using Crumbs.EFCore.EventualConsistency;
using Crumbs.EFCore.Session;
using Microsoft.EntityFrameworkCore;
using System;

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
            configurator.RegisterTransientType<IEventHandlerStateStore, EventHandlerStateStore>();

            configurator.RegisterSingelton<IDataStoreConnectionFactory, DataStoreConnectionFactory>();
            configurator.RegisterSingelton<IFrameworkContextFactory, DataStoreConnectionFactory>();
            // Todo: EventHandler state store

            return configurator;
        }

        public static FrameworkConfigurator UseScopedContext<TContextInterface>(
            this FrameworkConfigurator configurator,
            Func<DbContextOptions, TContextInterface> factoryMethod)
            where TContextInterface : IScopedContex
        {
            configurator.RegisterSingelton<
                ISessionScopedContextFactory<TContextInterface>,
                SessionScopedContextFactory<TContextInterface>>();

            configurator.RegisterInitializationAction(async resolver =>
            {
                var factory = resolver.Resolve<ISessionScopedContextFactory<TContextInterface>>();
                factory.Initialize(factoryMethod);
            });

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
            return configurator.RegisterInitializationAction(async resolver =>
            {
                var factory = resolver.Resolve<IFrameworkContextFactory>();

                using (var context = await factory.CreateContext())
                {
                    context.Migrate();
                }
            });
        }
    }
}
