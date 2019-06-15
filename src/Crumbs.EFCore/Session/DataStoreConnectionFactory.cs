using Crumbs.Core.Configuration;
using Crumbs.Core.DependencyInjection;
using Crumbs.Core.Exceptions;
using Crumbs.Core.Extensions;
using Crumbs.Core.Session;
using Crumbs.EFCore.Extensions;
using Crumbs.EFCore.ProviderContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Crumbs.EFCore.Session
{
    public class DataStoreConnectionFactory : IDataStoreConnectionFactory, IFrameworkContextFactory
    {
        public const string SqliteConnectionStringKey = nameof(SqliteConnectionStringKey);
        public const string MySqlConnectionStringKey = nameof(MySqlConnectionStringKey);
        public const string ProviderTypeKey = nameof(ProviderTypeKey);

        private ProviderType _providerType;
        private readonly IResolver _resolver;
        private readonly ISessionManager _sessionManager;

        // Todo: Get from config in providers
        public static string ConnectionString;

        public DataStoreConnectionFactory(
            IFrameworkConfiguration configuration,
            IResolver resolver,
            ISessionManager sessionManager)
        {
            _providerType = configuration.GetValue<ProviderType>(ProviderTypeKey);
            ConnectionString = GetConnectionString(_providerType, configuration);
            _resolver = resolver;
            _sessionManager = sessionManager;
        }

        private static string GetConnectionString(ProviderType providerType, IFrameworkConfiguration configuration)
        {
            switch (providerType)
            {
                case ProviderType.Sqlite:
                    return configuration.GetValue<string>(SqliteConnectionStringKey);
                case ProviderType.MySql:
                    return configuration.GetValue<string>(MySqlConnectionStringKey);
            }

            throw new FrameworkConfigurationException("Unknown provider.");
        }

        public async Task<IDataStoreConnection> Connect()
        {
            var connectionWrapper = new DatabaseConnectionWrapper(_resolver.Resolve<IFrameworkContext>());
            await connectionWrapper.OpenConnection();

            return connectionWrapper;
        }

        public async Task<IFrameworkContext> CreateContext(Guid? session = null)
        {
            if (session == null)
            {
                return _resolver.Resolve<IFrameworkContext>();
            }

            var scope = await _sessionManager.GetScopeForSession(session.Value);

            return CreateContext(_providerType, scope);
        }

        private IFrameworkContext CreateContext(ProviderType providerType, IDataStoreScope scope)
        {
            var options = new DbContextOptionsBuilder<FrameworkContext>();
            var connection = scope.AsDbConnection();
            IFrameworkContext context = null;

            switch (providerType)
            {
                case ProviderType.Sqlite:
                    context = new SqliteDbContext(options.UseSqlite(connection).Options);
                    break;
                case ProviderType.MySql:
                    context = new MySqlDbContext(options.UseMySql(connection).Options);
                    break;
            }

            if (context == null)
            {
                throw new Exception("Todo");
            }

            return context.UseTransaction(scope.AsTransaction());
        }
    }
}