using System;

namespace Crumbs.Core.Exceptions
{
    public class AggregateOrEventMissingIdException : Exception
    {
        public AggregateOrEventMissingIdException(Type aggregateType, Type eventType)
            : base($"Error while trying to save event of type {eventType.FullName} from {aggregateType.FullName}. Missing id on event or aggregate.")
        { }
    }
}