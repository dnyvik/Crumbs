using System;

namespace Crumbs.Core.Exceptions
{
    public class ConcurrencyException : Exception
    {
        public Guid EntityId { get; }

        public ConcurrencyException(Guid id)
            : base($"A different version than expected was found in aggregate {id}")
        {
            EntityId = id;
        }
    }
}