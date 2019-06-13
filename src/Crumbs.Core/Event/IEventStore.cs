using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crumbs.Core.Event
{
    // Todo: Add async postfix + consider streaming?
    public interface IEventStore
    {
        Task<IReadOnlyCollection<IDomainEvent>> Get(Guid aggregateId);
        Task<IReadOnlyCollection<IDomainEvent>> Get(Guid aggregateId, int fromVersion);
        Task<IReadOnlyCollection<IDomainEvent>> Get(Guid aggregateId, DateTimeOffset fromDate);
        Task<IReadOnlyCollection<IDomainEvent>> GetAll();
        Task<IReadOnlyCollection<IDomainEvent>> GetAllAfter(int eventId, int? batchSize = null);
        Task<IReadOnlyCollection<IDomainEvent>> Get(Guid aggregateId, int page, int itemsPerPage);
        Task<IEnumerable<IDomainEvent>> Save(IEnumerable<IDomainEvent> events, Guid? sessionKey = null);

        /// <summary>
        /// Not a thing in ES i know :P We can call this the GDRP method.
        /// Could consider scrambling events instead?
        /// </summary>
        Task Delete(Guid aggregateId);
    }
}