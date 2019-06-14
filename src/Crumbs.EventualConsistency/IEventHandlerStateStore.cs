using System;
using System.Threading.Tasks;

namespace Crumbs.EventualConsistency
{
    // Todo: Rewrite? Uses model interface instead of container?
    public interface IEventHandlerStateStore
    {
        Task<StateContainer<T>> Get<T>(Guid stateId) where T : EventHandlerState;
        Task Save<T>(StateContainer<T> stateContainer) where T : EventHandlerState;
        Task Delete(Guid stateId);
    }
}
