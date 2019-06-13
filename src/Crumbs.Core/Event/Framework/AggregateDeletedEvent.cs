using System;

namespace Crumbs.Core.Event.Framework
{
    public class AggregateDeletedEvent : DomainEvent
    {
        public AggregateDeletedEvent(Guid aggregateId, Guid sessionId, Guid userId)
        {
            AggregateId = aggregateId;
            SessionKey = sessionId;
            AppliedByUserId = userId;
            Timestamp = DateTimeOffset.UtcNow;
        }
    }
}