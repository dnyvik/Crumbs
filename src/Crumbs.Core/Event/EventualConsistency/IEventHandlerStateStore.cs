using System;
using System.Threading.Tasks;

namespace Crumbs.Core.Event.EventualConsistency
{
    public interface IEventHandlerStateStore
    {
        Task<IStateContainer<T>> Get<T>(Guid stateId) where T : IEventHandlerState;
        Task Save<T>(IStateContainer<T> stateContainer) where T : IEventHandlerState;
        Task Delete(Guid stateId);
    }
}
