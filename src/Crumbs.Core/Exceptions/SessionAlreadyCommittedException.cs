using System;

namespace Crumbs.Core.Exceptions
{
    public class SessionAlreadyCommittedException : Exception
    {
        public SessionAlreadyCommittedException(Guid sessionKey)
            : base($"Session '{sessionKey}' has already been committed.") { }
    }
}