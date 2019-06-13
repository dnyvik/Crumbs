using System;

namespace Crumbs.History
{
    public class HistoryEntryDto
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public string TriggeredByUser { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
