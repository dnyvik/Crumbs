using System;
using System.Collections.Generic;
using Crumbs.Core.Event;

namespace Crumbs.Core.Aggregate
{
    public interface IAggregateRoot : IEntity
    {
        bool HasPendingChanges { get; }
        int Version { get; }
        IEnumerable<IDomainEvent> GetUncommittedEvents();
        void LoadFromHistory(IEnumerable<IDomainEvent> history);
        IEnumerable<IDomainEvent> FlushUncommitedEvents(Guid? sessionKey, Guid userId);
    }
}