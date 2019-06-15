using Crumbs.Core.Event;
using Crumbs.EFCore.Models;
using Crumbs.EFCore.Session;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crumbs.EFCore
{
    // Todo: Compiled query for all read methods
    public class EventStore : IEventStore
    {
        private readonly IEventSerializer _eventSerializer;
        private readonly IFrameworkContextFactory _frameworkContextFactory;

        public EventStore(
            IEventSerializer eventSerializer,
            IFrameworkContextFactory frameworkContextFactory)
        {
            _eventSerializer = eventSerializer;
            _frameworkContextFactory = frameworkContextFactory;
        }

        // GDRP method - Consider scrambling events instead?
        public async Task Delete(Guid aggregateId)
        {
            using (var context = await _frameworkContextFactory.CreateContext())
            {
                var eventsToDelete = await context.Events
                                            .Where(e => e.AggregateId == aggregateId)
                                            .ToListAsync();

                context.Events.RemoveRange(eventsToDelete);

                await context.SaveChangesAsync(); // Todo: CT
            }
        }

        public async Task<IReadOnlyCollection<IDomainEvent>> Get(Guid aggregateId)
        {
            using (var context = await _frameworkContextFactory.CreateContext())
            {
                var eventDtos = await context.Events
                    .Where(e => e.AggregateId == aggregateId)
                    .OrderBy(e => e.AggregateVersion)
                    .AsNoTracking()
                    .ToListAsync();

                return eventDtos.Select(Deserialize).ToList().AsReadOnly();
            }
        }

        public async Task<IReadOnlyCollection<IDomainEvent>> Get(Guid aggregateId, int fromVersion)
        {
            using (var context = await _frameworkContextFactory.CreateContext())
            {
                var eventDtos = await context.Events
                    .Where(e => e.AggregateId == aggregateId && e.AggregateVersion > fromVersion)
                    .OrderBy(e => e.AggregateVersion)
                    .AsNoTracking()
                    .ToListAsync();

                return eventDtos.Select(Deserialize).ToList().AsReadOnly();
            }
        }

        public Task<IReadOnlyCollection<IDomainEvent>> Get(Guid aggregateId, DateTimeOffset fromDate)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IDomainEvent>> Get(Guid aggregateId, int page, int itemsPerPage)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IDomainEvent>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<IDomainEvent>> GetAllAfter(long eventId, int? batchSize = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IDomainEvent>> Save(IEnumerable<IDomainEvent> events, Guid? sessionKey = null)
        {
            using (var context = await _frameworkContextFactory.CreateContext(sessionKey))
            {
                var domainEvents = events as IList<IDomainEvent> ?? events.ToList();

                var eventData = domainEvents
                    .Select(e =>
                            new
                            {
                                DomainEvent = e,
                                Entity = CreateEntity(e)
                            })
                    .ToList();

                context.Events.AddRange(eventData.Select(x => x.Entity));
                await context.SaveChangesAsync(); // Todo: CT

                foreach (var d in eventData)
                {
                    d.DomainEvent.Id = d.Entity.EventId;
                }

                // Todo: Need ordering?
                return eventData.Select(x => x.DomainEvent);
            }
        }

        private Event CreateEntity(IDomainEvent e)
        {
            return new Event
            {
                AggregateId = e.AggregateId,
                AppliedByUserId = e.AppliedByUserId,
                // ReSharper disable once PossibleInvalidOperationException
                // All events are eventually given a session key and this is therefore safe.
                SessionId = e.SessionKey.Value,
                AggregateVersion = e.Version,
                Type = e.GetType().AssemblyQualifiedName,
                Timestamp = e.Timestamp,
                Data = SerializeEvent(e)
            };
        }

        private IDomainEvent Deserialize(Event entity)
        {
            var domainEvent = _eventSerializer.Deserialize(entity.Data, Type.GetType(entity.Type));

            domainEvent.AggregateId = entity.AggregateId;
            domainEvent.AppliedByUserId = entity.AppliedByUserId;
            domainEvent.Version = entity.AggregateVersion;
            domainEvent.Timestamp = entity.Timestamp;
            domainEvent.Id = entity.EventId;

            return domainEvent;
        }

        private string SerializeEvent(IDomainEvent domainEvent)
        {
            return _eventSerializer.Serialize(domainEvent);
        }
    }
}
