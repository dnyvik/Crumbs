using Crumbs.Core.Aggregate;

namespace Crumbs.Core.Session
{
    public class AggregateDescriptor
    {
        public IAggregateRoot Aggregate { get; set; }
        public int Version { get; set; }
        public bool IsMarkedForDeletion { get; set; }
    }
}