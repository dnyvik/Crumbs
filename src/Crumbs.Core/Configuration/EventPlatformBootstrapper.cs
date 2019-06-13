using System;

namespace Crumbs.Core.Configuration
{
    public static class EventPlatformBootstrapper
    {
        private static bool _initialized;

        public static FrameworkConfigurator Configure()
        {
            if (_initialized)
                throw new InvalidOperationException("Framework has already been initialized.");

            return new FrameworkConfigurator(configuration =>
            {
                // Todo: Do actual init logic from here?
                _initialized = true;
            });
        }
    }
}
