using System;

namespace Crumbs.Core.Configuration.SessionProfiles
{
    public class PatientLoadProfile : ISessionProfile
    {
        public int MaxLoadAttempts => 30;
        public TimeSpan MinBackoffBetweenLoadAttempts => TimeSpan.FromMilliseconds(25);
        public TimeSpan MaxBackoffBetweenLoadAttempts => TimeSpan.FromMilliseconds(50);
        public TimeSpan LoadAttemptTimeout => TimeSpan.FromMilliseconds(250);
    }
}
