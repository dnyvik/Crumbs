using System;

namespace Crumbs.Core.Exceptions
{
    public class MaxRetryLimitExceededException : Exception
    {
        public MaxRetryLimitExceededException(int retryLimit, Guid aggregateId, Type aggregateType)
            : base($"Max retry attempts reached ({retryLimit}) for loading aggregate with id '{aggregateId}' and type '{aggregateType}' into session.") { }
    }
}