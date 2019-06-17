using System;

namespace Crumbs.Core.Configuration.SessionProfiles
{
    public class FillOrKillProfile : ISessionProfile
    {
        public int MaxLoadAttempts => 1;
        public TimeSpan MinBackoffBetweenLoadAttempts => TimeSpan.Zero;
        public TimeSpan MaxBackoffBetweenLoadAttempts => TimeSpan.Zero;
        public TimeSpan LoadAttemptTimeout => TimeSpan.FromMilliseconds(100);
    }
}
