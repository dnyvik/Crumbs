using System;

namespace Crumbs.Core.Exceptions
{
    /// <summary>
    /// Base exception for domain exceptions.
    /// </summary>
    public class DomainException : InvalidOperationException
    {
        public DomainException(string message) : base(message) { }

        // Todo: Create more static helpers.
        public static DomainException Error(string message)
        {
            return new DomainException(message);
        }
    }
}