using Crumbs.Core.Event.EventualConsistency;
using System;
using System.Threading.Tasks;

namespace Crumbs.EventualConsistency
{
    public interface IStatefulEventHandler<T> where T : IEventHandlerState
    {
        Task Initialize(Guid stateKey, T initalState = default, bool useLiveEventStream = true);
        T State { get; set; } // Todo: Refactor public setter
        Task Delete();
        bool IsFaulted { get; }
    }
}
