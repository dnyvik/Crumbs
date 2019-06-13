using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crumbs.History
{
    public interface IHistoryService
    {
        Task<IEnumerable<HistoryEntryDto>> Get(Guid aggregateId);
        Task<IEnumerable<HistoryEntryDto>> Get(Guid aggregateId, int fromVersion);
        Task<IEnumerable<HistoryEntryDto>> Get(Guid aggregateId, DateTimeOffset fromDate);
        Task<IEnumerable<HistoryEntryDto>> Get(Guid id, int page, int itemsPerPage);
    }
}
