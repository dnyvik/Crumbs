using Crumbs.Core.Event;
using System;
using System.Threading.Tasks;

namespace Crumbs.EFCore.Session
{
    public abstract class SessionScopedDomainEventHandler<TContextInterface> 
        where TContextInterface : IScopedContex
    {
        private readonly ISessionScopedContextFactory<TContextInterface> _contextFactory;

        protected SessionScopedDomainEventHandler(
            ISessionScopedContextFactory<TContextInterface> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        protected async Task<TContextInterface> CreateContext(IDomainEvent domainEvent)
        {
            if (!domainEvent.SessionKey.HasValue)
                throw new InvalidOperationException("Missing session key.");

            return await _contextFactory.CreateContext<TContextInterface>(domainEvent.SessionKey);
        }
    }
}

