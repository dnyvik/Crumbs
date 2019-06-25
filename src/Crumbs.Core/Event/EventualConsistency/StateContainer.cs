using System;

namespace Crumbs.Core.Event.EventualConsistency
{
    public class StateContainer<T> : IStateContainer<T> where T : IEventHandlerState
    {
        public Guid Id { get; set; }
        public long ProcessedEventId { get; set; }
        public T State { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public StatefulHandlerStatus HandlerStatus { get; set; }
    }
}
