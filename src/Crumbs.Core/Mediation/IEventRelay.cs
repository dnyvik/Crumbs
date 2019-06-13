using System;
using Crumbs.Core.Event;

namespace Crumbs.Core.Mediation
{
    public interface IEventRelay
    {
        // Todo: Async?
        void RegisterRelayHandler(Action<IDomainEvent> handler);
    }
}