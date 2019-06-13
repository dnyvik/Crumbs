using System;
using System.Collections.Generic;
using System.Linq;
using Crumbs.Core.Event;
using Crumbs.Core.Exceptions;

namespace Crumbs.Core.Aggregate
{
    public abstract class AggregateRoot : IAggregateRoot
    {
        public Guid Id { get; protected set; }
        public int Version { get; protected set; }

        private readonly IList<IDomainEvent> _uncommittedEvents = new List<IDomainEvent>();

        Guid IEntity.Id => Id;

        bool IAggregateRoot.HasPendingChanges
        {
            get
            {
                return _uncommittedEvents.Any();
            }
        }

        IEnumerable<IDomainEvent> IAggregateRoot.GetUncommittedEvents()
        {
            return _uncommittedEvents.ToArray();
        }

        IEnumerable<IDomainEvent> IAggregateRoot.FlushUncommitedEvents(Guid? sessionKey, Guid userId)
        {
            var events = _uncommittedEvents.ToArray();
            var i = 0;
            var timestamp = DateTimeOffset.UtcNow;

            foreach (var @event in events)
            {
                if (@event.AggregateId == Guid.Empty && Id == Guid.Empty)
                {
                    throw new AggregateOrEventMissingIdException(GetType(), @event.GetType());
                }
                if (@event.AggregateId == Guid.Empty)
                {
                    @event.AggregateId = Id;
                }
                if (sessionKey.HasValue)
                {
                    @event.SessionKey = sessionKey.Value;
                }
                i++;
                @event.Version = Version + i;
                @event.Timestamp = timestamp;
                @event.AppliedByUserId = userId;
            }
            Version = Version + _uncommittedEvents.Count;
            _uncommittedEvents.Clear();
            return events;
        }

        protected void RaiseEvent(IDomainEvent e)
        {
            RaiseEvent(e, true);
        }

        void IAggregateRoot.LoadFromHistory(IEnumerable<IDomainEvent> history)  
        {
            foreach (var e in history.OrderBy(e => e.Version))
            {
                if (e.Version != Version + 1)
                {
                    throw new EventsOutOfOrderException(e.AggregateId);
                }
                RaiseEvent(e, false);
            }
        }

        private void RaiseEvent(IDomainEvent e, bool isNewEvent)
        {
            (this as dynamic).Apply((dynamic)e);
            if (isNewEvent)
            {
                _uncommittedEvents.Add(e);
            }
            else
            {
                Id = e.AggregateId;
                Version++;
            }
        }
    }
}