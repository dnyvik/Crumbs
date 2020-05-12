using Crumbs.Core.Event;
using Crumbs.Core.Event.EventualConsistency;

namespace Crumbs.EventualConsistency
{
    public abstract class EventuallyConsistentEventHandler : StatefulEventuallyConsistentEventHandler<EventHandlerState>
    {
        protected EventuallyConsistentEventHandler(
            IEventStore eventStore,
            IEventStreamer eventStreamer,
            IEventHandlerStateStore eventHandlerStateStore) :
            base(eventStore, eventStreamer, eventHandlerStateStore)
        {
            State.HandlerType = this.GetType().FullName;
        }
    }

    public class EventHandlerState : IEventHandlerState
    {
        public string HandlerType { get; set; } // Todo: Move to historical event handler schema?
    }
}
