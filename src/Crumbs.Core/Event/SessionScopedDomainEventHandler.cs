using System;
using System.Threading.Tasks;
using Crumbs.Core.Event;
using Crumbs.Core.Exceptions;
using Crumbs.Core.Session;

namespace Crumbs.Core
{
    public abstract class SessionScopedDomainEventHandler
    {
        private readonly ISessionManager _sessionManager;

        protected SessionScopedDomainEventHandler(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        protected async Task<IDataStoreScope> GetSessionScope(IDomainEvent domainEvent)
        {
            if (!domainEvent.SessionKey.HasValue)
                throw new InvalidOperationException("Missing session key.");

            var scopeForSession = await _sessionManager.GetScopeForSession(domainEvent.SessionKey.Value);
            if (scopeForSession == null)
                throw new MissingDataStoreScopeException(domainEvent.SessionKey.Value);

            return scopeForSession;
        }
    }
}
