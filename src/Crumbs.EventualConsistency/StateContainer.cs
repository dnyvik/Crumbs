using System;

namespace Crumbs.EventualConsistency
{
    public class StateContainer<T>
    {
        public Guid Id { get; set; }
        public int ProcessedEventId { get; set; }
        public T State { get; set; }

        // Todo: Abstraction
        public HandlerStatus HandlerStatus { get; set; }
    }
}
