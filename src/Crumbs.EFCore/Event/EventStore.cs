using Crumbs.Core.Event;
using Crumbs.EFCore.Session;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crumbs.EFCore.Event
{
    // Todo: Compiled query for all read methods
    public class EventStore : IEventStore
    {
        private const int MaximumRowsPerQueryLimit = 10000; // Todo: Make configurable or set per provider?
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
                    .ToListAsync(); // Todo: CT

                return eventDtos
                    .Select(Deserialize)
                    .ToList()
                    .AsReadOnly();
            }
        }

        // Todo: Implement for alle getters
        public async Task<IReadOnlyCollection<T>> Get<T>(Guid aggregateId) where T : IDomainEvent
        {
            using (var context = await _frameworkContextFactory.CreateContext())
            {
                var eventType = typeof(T).AssemblyQualifiedName;

                var eventDtos = await context.Events
                    .Where(e => e.AggregateId == aggregateId)
                    .Where(e => e.Type == eventType)
                    .OrderBy(e => e.AggregateVersion)
                    .AsNoTracking()
                    .ToListAsync(); // Todo: CT

                return eventDtos
                    .Select(Deserialize)
                    .Cast<T>()
                    .ToList()
                    .AsReadOnly();
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
                    .ToListAsync(); // Todo: CT

                return eventDtos
                    .Select(Deserialize)
                    .ToList()
                    .AsReadOnly();
            }
        }

        public async Task<IReadOnlyCollection<IDomainEvent>> Get(Guid aggregateId, DateTimeOffset fromDate)
        {
            using (var context = await _frameworkContextFactory.CreateContext())
            {
                var eventDtos = await context.Events
                                    .Where(e => e.AggregateId == aggregateId 
                                            && e.Timestamp > fromDate)
                                    .OrderBy(e => e.AggregateVersion)
                                    .ToListAsync();

                return eventDtos.Select(Deserialize).ToList().AsReadOnly();
            }
        }

        public async Task<IReadOnlyCollection<IDomainEvent>> Get(Guid aggregateId, int page, int itemsPerPage)
        {
            if (itemsPerPage > MaximumRowsPerQueryLimit)
            {
                throw new ArgumentException($"Maximum rows per query limit exceeded. " +
                    $"Items per page needs to be '{MaximumRowsPerQueryLimit}' or below.");
            }

            using (var context = await _frameworkContextFactory.CreateContext())
            {
                var itemsToSkip = (page - 1) * itemsPerPage;

                var events = await context.Events
                                         .Where(e => e.AggregateId == aggregateId)
                                         .OrderByDescending(e => e.AggregateVersion)
                                         .Skip(itemsToSkip)
                                         .Take(itemsPerPage)
                                         .AsNoTracking()
                                         .ToListAsync();

                return events.Select(Deserialize).ToList().AsReadOnly();
            }
        }

        public async Task<IReadOnlyCollection<IDomainEvent>> GetAllAfter(long eventId, int batchSize)
        {
            if (batchSize > MaximumRowsPerQueryLimit)
            {
                throw new ArgumentException($"Maximum rows per query limit exceeded. " +
                    $"Batch size needs to be '{MaximumRowsPerQueryLimit}' or below.");
            }

            using (var context = await _frameworkContextFactory.CreateContext())
            {
                var events = await context.Events
                    .AsNoTracking()
                    .Where(e => e.EventId > eventId)
                    .Take(batchSize)
                    .OrderBy(e => e.EventId) // Todo: Need?
                    .ToListAsync(); // Todo: CT

                return events
                    .Select(Deserialize)
                    .ToList()
                    .AsReadOnly();
            }
        }

        public async Task<IEnumerable<IDomainEvent>> Save(IEnumerable<IDomainEvent> events, Guid? sessionKey = null)
        {
            using (var context = await _frameworkContextFactory.CreateContext(sessionKey))
            {
                var domainEvents = events as IList<IDomainEvent> ?? events.ToList();

                var eventData = domainEvents.Select(e =>
                                new
                                {
                                    DomainEvent = e,
                                    Entity = CreateEntity(e)
                                });

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

        private Models.Event CreateEntity(IDomainEvent e)
        {
            return new Models.Event
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

        private IDomainEvent Deserialize(Models.Event entity)
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
