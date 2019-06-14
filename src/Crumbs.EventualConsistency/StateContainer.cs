using Crumbs.Core.Event.EventualConsistency;
using System;

namespace Crumbs.EventualConsistency
{
    public class StateContainer<T> : IStateContainer<T> where T : IEventHandlerState
    {
        public Guid Id { get; set; }
        public int ProcessedEventId { get; set; }
        public T State { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public StatefulHandlerStatus HandlerStatus { get; set; }
    }
}
