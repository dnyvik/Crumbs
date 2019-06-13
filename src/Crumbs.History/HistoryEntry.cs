using System;

namespace Crumbs.History
{
    public class HistoryEntry : Attribute
    {
        public HistoryEntry() { }

        public HistoryEntry(string label)
        {
            Label = label;
        }

        public string Label { get; }
    }
}
