using System;
using System.Runtime.Serialization;

namespace Crumbs.Core.Snapshot
{
    /// <summary>
    /// Base class for aggregate snapshot types. Extend this class with a type that has the aggregate specific properties.
    /// An aggregate is stored in serialized form, but the properties in this base class will be set explicitly rather
    /// than implicitly when deserializing.
    /// </summary>
    public abstract class Snapshot
    {
        /// <summary>
        /// Unique identifier (across aggregate types) for aggregate.
        /// </summary>
        /// <remarks>
        /// Not serialized, because the relational providers uses this as its primary key.
        /// </remarks>
        [IgnoreDataMember]
        public Guid AggregateId { get; set; }

        /// <remarks>
        /// Not serialized, because the relational providers stores this field explicitly.
        /// </remarks>
        [IgnoreDataMember]
        public int Version { get; set; }
    }
}