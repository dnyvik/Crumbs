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

            configurator.SetDependencyFramework(wrapper);

            return configurator;
        }
    }
}
