using System;

namespace Crumbs.Core.Configuration.SessionProfiles
{
    public class DefaultSessionProfile : ISessionProfile
    {
        public int MaxLoadAttempts => 15;
        public TimeSpan MinBackoffBetweenLoadAttempts => TimeSpan.FromMilliseconds(25);
        public TimeSpan MaxBackoffBetweenLoadAttempts => TimeSpan.FromMilliseconds(100);
        public TimeSpan LoadAttemptTimeout => TimeSpan.FromMilliseconds(100);
    }
}
