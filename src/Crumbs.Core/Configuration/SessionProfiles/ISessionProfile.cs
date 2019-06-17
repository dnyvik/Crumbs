using System;

namespace Crumbs.Core.Configuration.SessionProfiles
{
    public interface ISessionProfile
    {
        int MaxLoadAttempts { get; }
        TimeSpan MinBackoffBetweenLoadAttempts { get; }
        TimeSpan MaxBackoffBetweenLoadAttempts { get; }
        TimeSpan LoadAttemptTimeout { get; }
    }
}
