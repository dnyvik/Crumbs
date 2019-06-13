using System;

namespace Crumbs.Core.Exceptions
{
    public class MissingDataStoreScopeException : Exception
    {
        public MissingDataStoreScopeException(Guid sessionKey)
            : base($"Data store scope for session '{sessionKey}' is missing.") { }
    }
}
