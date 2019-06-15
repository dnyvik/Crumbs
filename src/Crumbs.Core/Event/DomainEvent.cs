using System;
using System.Runtime.Serialization;

namespace Crumbs.Core.Event
{
    public abstract class DomainEvent : IDomainEvent
    {
        [IgnoreDataMember]
        public long Id { get; set; }
        [IgnoreDataMember]
        public Guid AppliedByUserId { get; set; }
        [IgnoreDataMember]
        public Guid AggregateId { get; set; } // Set in event store
        [IgnoreDataMember]
        public int Version { get; set; } // Set in event store
        [IgnoreDataMember]
        public DateTimeOffset Timestamp { get; set; } // Set in event store

        /// <summary>
        /// Session key, used by event handlers to persist in same transaction scope.
        /// </summary>
        [IgnoreDataMember]
        public Guid? SessionKey { get; set; }
    }
}