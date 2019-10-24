using System;
using System.Threading.Tasks;

namespace Crumbs.Core.Configuration
{
    public static class CrumbsBootstrapper
    {
        private static FrameworkConfigurator _configurator;
        public static FrameworkConfigurator Configure()
        {
            if (_configurator != null)
            {
                return _configurator;
            }

            _configurator = new FrameworkConfigurator(configuration =>
            {
                _configurator = null; //Todo: Dispose
                // Todo: Do actual init logic from here?
            });

            return _configurator;
        }
    }
}
