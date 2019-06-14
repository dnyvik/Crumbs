using Crumbs.Core.Event;
using Crumbs.Core.Mediation;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Crumbs.EventualConsistency
{
    public class EventStreamer : IEventStreamer
    {
        public EventStreamer(IEventRelay eventRelay)
        {
            var subject = new Subject<IDomainEvent>();

            eventRelay.RegisterRelayHandler(e => subject.OnNext(e));

            EventStream = subject.AsObservable()
                .ObserveOn(NewThreadScheduler.Default) // Events published to stream from relay should be non blocking. Defer to IEventHandler for blocking calls.
                .Publish();
        }

        public IConnectableObservable<IDomainEvent> EventStream { get; }
    }
}
