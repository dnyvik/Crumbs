using System;
using System.Runtime.Serialization;

namespace Crumbs.EventualConsistency
{
    public abstract class EventHandlerState
    {
        [IgnoreDataMember]
        public DateTimeOffset LastUpdated { get; set; }
    }
}
