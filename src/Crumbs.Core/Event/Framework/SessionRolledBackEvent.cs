using System;

namespace Crumbs.Core.Event.Framework
{
    public class SessionRolledBackEvent : DomainEvent
    {
        public SessionRolledBackEvent(Guid sessionId, Guid userId)
        {
            SessionKey = sessionId;
            AppliedByUserId = userId;
            Timestamp = DateTimeOffset.UtcNow;
        }
    }
}