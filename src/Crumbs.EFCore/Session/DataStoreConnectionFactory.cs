using Crumbs.Core.Configuration;
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
        public const string EFCoreConnectionStringKey = nameof(EFCoreConnectionStringKey);
        public const string ProviderTypeKey = nameof(ProviderTypeKey);

        private readonly ProviderType _providerType;
        private readonly string ConnectionString;
        private readonly ISessionManager _sessionManager;

        public DataStoreConnectionFactory(
            IFrameworkConfiguration configuration,
            ISessionManager sessionManager)
        {
            _providerType = configuration.GetValue<ProviderType>(ProviderTypeKey);
            ConnectionString = configuration.GetValue<string>(EFCoreConnectionStringKey);
            _sessionManager = sessionManager;
        }

        public async Task<IDataStoreConnection> Connect()
        {
            var connectionWrapper = new DatabaseConnectionWrapper(await CreateContext());
            await connectionWrapper.OpenConnection();

            return connectionWrapper;
        }

        public async Task<IFrameworkContext> CreateContext(Guid? session = null)
        {
            var scope = session.HasValue ? 
                await _sessionManager.GetScopeForSession(session.Value) :
                null;

            return CreateContext(_providerType, scope);
        }

        private IFrameworkContext CreateContext(ProviderType providerType, IDataStoreScope scope)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FrameworkContext>();
            IFrameworkContext context = null;

            switch (providerType)
            {
                case ProviderType.Sqlite:
                    var sqliteOptions = scope == null ?
                        optionsBuilder.UseSqlite(ConnectionString).Options :
                        optionsBuilder.UseSqlite(scope.AsDbConnection()).Options;
                    context = new SqliteDbContext(sqliteOptions);
                    break;
                case ProviderType.MySql:
                    var mySqlOptions = scope == null ?
                        optionsBuilder.UseMySql(ConnectionString).Options :
                        optionsBuilder.UseMySql(scope.AsDbConnection()).Options;
                    context = new MySqlDbContext(mySqlOptions);
                    break;
                default:
                    throw new NotImplementedException($"No provider exists for '{providerType}'.");
            }

            return scope != null ?
                context.UseTransaction(scope.AsTransaction()) :
                context;
        }
    }
}