using Crumbs.Core.Event.EventualConsistency;
using System;
using System.Threading.Tasks;

namespace Crumbs.EventualConsistency
{
    public interface IEventHandlerStateStore
    {
        Task<StateContainer<T>> Get<T>(Guid stateId) where T : IEventHandlerState;
        Task Save<T>(StateContainer<T> stateContainer) where T : IEventHandlerState;
        Task Delete(Guid stateId);
    }
}
