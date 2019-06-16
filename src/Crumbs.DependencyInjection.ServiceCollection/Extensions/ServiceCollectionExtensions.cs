using Crumbs.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Crumbs.DependencyInjection.ServiceCollection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCrumbs(
            this IServiceCollection services,
            Action<FrameworkConfigurator> configuratorAction)
        {
            var configurator = CrumbsBootstrapper.Configure()
                .UseServiceCollection(services);

            configuratorAction(configurator);
            configurator.Run();

            return services;
        }
    }
}
