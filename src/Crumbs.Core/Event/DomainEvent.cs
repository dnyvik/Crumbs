using System;
using System.Runtime.Serialization;

namespace Crumbs.Core.Event
{
    public abstract class DomainEvent : IDomainEvent
    {
        [IgnoreDataMember]
        public long Id { get; set; } // Set by event store
        [IgnoreDataMember]
        public Guid AppliedByUserId { get; set; }
        [IgnoreDataMember]
        public Guid AggregateId { get; set; } // Set on flush
        [IgnoreDataMember]
        public int Version { get; set; } // Set on flush
        [IgnoreDataMember]
        public DateTimeOffset Timestamp { get; set; } // Set on flush

        /// <summary>
        /// Session key, used by event handlers to persist in same transaction scope.
        /// </summary>
        [IgnoreDataMember]
        public Guid? SessionKey { get; set; }
    }
}