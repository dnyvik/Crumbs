using Crumbs.Core.Mediation;
using System;

namespace Crumbs.Core.Event
{
    public interface IDomainEvent : IMessage
    {
        long Id { get; set; }
        Guid AppliedByUserId { get; set; }
        Guid AggregateId { get; set; }
        int Version { get; set; }
        DateTimeOffset Timestamp { get; set; }
        Guid? SessionKey { get; set; }
    }
}