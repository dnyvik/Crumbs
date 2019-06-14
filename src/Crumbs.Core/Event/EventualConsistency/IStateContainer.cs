using System;

namespace Crumbs.Core.Event.EventualConsistency
{
    public interface IStateContainer<T> where T : IEventHandlerState
    {
        StatefulHandlerStatus HandlerStatus { get; set; }
        Guid Id { get; set; }
        DateTimeOffset LastUpdated { get; set; }
        int ProcessedEventId { get; set; }
        T State { get; set; }
    }
}