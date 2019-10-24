using Crumbs.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crumbs.DependencyInjection.ServiceCollection.Extensions
{
    public static class FrameworkConfiguratorExtensions
    {
        public static FrameworkConfigurator UseServiceCollection(
            this FrameworkConfigurator configurator,
            IServiceCollection services)
        {
            var wrapper = new ServiceCollectionWrapper(services);

            return configurator.SetDependencyFramework(wrapper);
        }

        internal static FrameworkConfigurator UseServiceCollectionRegistryWrapper(
            this FrameworkConfigurator configurator,
            IServiceCollection services)
        {
            var wrapper = new ServiceCollectionRegistryWrapper(services);
            return configurator.SetDependencyFramework(wrapper);
        }
    }
}
