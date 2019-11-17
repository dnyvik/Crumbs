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
            configurator.Run().Wait(TimeSpan.FromMinutes(1));

            return services;
        }

        // Todo: Split libs for console and ASP.NET core
        public static IServiceCollection AddCrumbsAspNetCore(
            this IServiceCollection services,
            Action<FrameworkConfigurator> configuratorAction)
        {
            var configurator = CrumbsBootstrapper.Configure()
                .UseServiceCollectionRegistryWrapper(services);

            configuratorAction(configurator);

            configurator.RegisterDependencies();

            return services;
        }
    }
}
