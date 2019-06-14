using Crumbs.EventualConsistency;
using System;
using System.Threading.Tasks;

namespace Crumbs.EventualConsistency
{
    public interface IStatefulEventHandler<T> where T : EventHandlerState
    {
        Task Initialize(Guid stateKey, T initalState = default(T), bool useLiveEventStream = true);
        T State { get; set; } // Todo: Remove public setter
        Task Delete();
        bool IsFaulted { get; }
    }
}
