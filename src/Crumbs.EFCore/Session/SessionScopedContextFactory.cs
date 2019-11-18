using Crumbs.Core.Configuration;
using Crumbs.Core.Extensions;
using Crumbs.Core.Session;
using Crumbs.EFCore.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Crumbs.EFCore.Session
{
    public class SessionScopedContextFactory<TContextInterface> :
        ISessionScopedContextFactory<TContextInterface>
        where TContextInterface : IScopedContex
    {
        public const string EFCoreConnectionStringKey = nameof(EFCoreConnectionStringKey);
        public const string ProviderTypeKey = nameof(ProviderTypeKey);

        private readonly ISessionManager _sessionManager;
        private readonly ProviderType _providerType;
        private readonly string _connectionString;

        private Func<DbContextOptions<SessionScopedContext>, TContextInterface> _factoryMethod;

        public SessionScopedContextFactory(
            ISessionManager sessionManager,
            IFrameworkConfiguration configuration)
        {
            _sessionManager = sessionManager;
            _providerType = configuration.GetValue<ProviderType>(ProviderTypeKey);
            _connectionString = configuration.GetValue<string>(EFCoreConnectionStringKey);
        }

        public void Initialize(Func<DbContextOptions<SessionScopedContext>, TContextInterface> factoryMethod)
        {
            _factoryMethod = factoryMethod;
        }

        public async Task<TContextInterface> CreateContext<TInterface>(Guid? session = null)
        {
            var scope = session.HasValue ?
                await _sessionManager.GetScopeForSession(session.Value) :
                null;

            return CreateContext(_providerType, scope);
        }

        private TContextInterface CreateContext(ProviderType providerType, IDataStoreScope scope)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SessionScopedContext>();
            TContextInterface context;

            switch (providerType)
            {
                case ProviderType.Sqlite:
                    var sqliteOptions = scope == null ?
                        optionsBuilder.UseSqlite(_connectionString).Options :
                        optionsBuilder.UseSqlite(scope.AsDbConnection()).Options;
                    context = _factoryMethod(sqliteOptions);
                    break;
                case ProviderType.MySql:
                    var mySqlOptions = scope == null ?
                        optionsBuilder.UseMySql(_connectionString).Options :
                        optionsBuilder.UseMySql(scope.AsDbConnection()).Options;
                    context = _factoryMethod(mySqlOptions);
                    break;
                default:
                    throw new NotImplementedException($"No provider exists for '{providerType}'.");
            }

            if (scope != null)
            {
                context.ScopeTo(scope);
                return context;
            }

            return default;
        }
    }
}

