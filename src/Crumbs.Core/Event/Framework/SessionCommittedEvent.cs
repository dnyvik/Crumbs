using System;

namespace Crumbs.Core.Event.Framework
{
    public class SessionCommittedEvent : DomainEvent
    {
        public SessionCommittedEvent(Guid sessionId, Guid userId)
        {
            SessionKey = sessionId;
            AppliedByUserId = userId;
            Timestamp = DateTimeOffset.UtcNow;
        }
    }
}