using Crumbs.Core.Event;
using System.Reactive.Subjects;

namespace Crumbs.EventualConsistency
{
    public interface IEventStreamer
    {
        IConnectableObservable<IDomainEvent> EventStream { get; }
    }
}
