using Crumbs.Core.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Crumbs.History
{
    // Todo: Configurator extension
    // Todo: Condier using async streams in the future
    public class HistoryService : IHistoryService
    {
        private static readonly HashSet<string> IgnoredProperties;
        private static readonly Type HistoryEntryAttributeType = typeof(HistoryEntry);
        private const string UnknownUserText = "Unknown";
        private const string EventPostfix = "Event";
        private const string ActionDetailsSeparator = ", ";
        private readonly IEventStore _eventStore;
        private readonly IUserDescriptor _userDescriptor;
        private readonly Dictionary<Guid, string> _userIdToNameMap = new Dictionary<Guid, string>();

        static HistoryService()
        {
            IgnoredProperties = new HashSet<string>(typeof(DomainEvent).GetProperties().Select(p => p.Name));
        }

        public HistoryService(IEventStore eventStore, IUserDescriptor userDescriptor)
        {
            _eventStore = eventStore;
            _userDescriptor = userDescriptor;
        }

        public async Task<IEnumerable<HistoryEntryDto>> Get(Guid aggregateId)
        {
            var events = await _eventStore.Get(aggregateId);

            return await ToDto(events);
        }

        public async Task<IEnumerable<HistoryEntryDto>> Get(Guid aggregateId, int fromVersion)
        {
            var events = await _eventStore.Get(aggregateId, fromVersion);

            return await ToDto(events);
        }

        public async Task<IEnumerable<HistoryEntryDto>> Get(Guid aggregateId, DateTimeOffset fromDate)
        {
            var events = await _eventStore.Get(aggregateId, fromDate);

            return await ToDto(events);
        }

        public async Task<IEnumerable<HistoryEntryDto>> Get(Guid id, int page, int itemsPerPage)
        {
            var events = await _eventStore.Get(id, page, itemsPerPage);

            return await ToDto(events);
        }

        protected HistoryEntryDto CreateHistoryEntry(IDomainEvent domainEvent, string performedByUser, string eventName = null, string eventDetails = null)
        {
            return new HistoryEntryDto
            {
                Name = GetEventName(eventName, domainEvent),
                Details = GetEventDetails(domainEvent),
                TriggeredByUser = performedByUser,
                Timestamp = domainEvent.Timestamp
            };
        }

        private async Task<IEnumerable<HistoryEntryDto>> ToDto(IEnumerable<IDomainEvent> domainEvents)
        {
            var entries = new List<HistoryEntryDto>();

            foreach (var domainEvent in domainEvents)
            {
                var nameOfUser = await GetNameOfUser(domainEvent.AppliedByUserId);
                entries.Add(CreateHistoryEntry(domainEvent, nameOfUser));
            }

            return entries;
        }

        private async Task<string> GetNameOfUser(Guid userId)
        {
            if (_userIdToNameMap.ContainsKey(userId))
            {
                return _userIdToNameMap[userId];
            }

            var userDescription = await _userDescriptor.GetUserDescription(userId);

            if (string.IsNullOrWhiteSpace(userDescription))
            {
                return UnknownUserText;
            }

            _userIdToNameMap[userId] = userDescription;

            return userDescription;
        }

        private string GetEventDetails(IDomainEvent e)
        {
            var properties = GetProperties(e)
                .Where(p => !IgnoredProperties.Contains(p.Name))
                .Select(p => new
                {
                    Property = p,
                    HistoryEntryAttribute = p.GetCustomAttributes(HistoryEntryAttributeType, true).FirstOrDefault()
                })
                .Where(x => x.HistoryEntryAttribute != null)
                .Select(x =>
                {
                    var attributeLabel = ((HistoryEntry)x.HistoryEntryAttribute).Label;
                    var label = string.IsNullOrWhiteSpace(attributeLabel) ? x.Property.Name : attributeLabel;
                    return $"{label}: {x.Property.GetValue(e, null)}";
                })
                .ToList();

            return properties.Any()
                ? string.Join(ActionDetailsSeparator, properties)
                : null;
        }

        private string GetEventName(string action, IDomainEvent e)
        {
            return string.IsNullOrWhiteSpace(action)
                       ? e.GetType().Name.Replace(EventPostfix, string.Empty).SplitCamelCase()
                       : action;
        }

        private static IEnumerable<PropertyInfo> GetProperties(object obj)
        {
            return obj.GetType().GetProperties();
        }
    }
}
