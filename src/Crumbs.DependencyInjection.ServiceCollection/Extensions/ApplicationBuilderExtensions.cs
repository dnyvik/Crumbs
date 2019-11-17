using Crumbs.Core.Configuration;
using Microsoft.AspNetCore.Builder;
using System;

namespace Crumbs.DependencyInjection.ServiceCollection.Extensions
{
    // Todo: Move into seperate lib for asp.net core
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCrumbs(this IApplicationBuilder applicationBuilder)
        {
            var wrapper = new ServiceProviderWrapper(applicationBuilder.ApplicationServices);

            var configuration = CrumbsBootstrapper.Configure()
                .SetDependencyFramework(wrapper);

            configuration.Initialize().Wait(TimeSpan.FromMinutes(1));

            return applicationBuilder;
        }
    }
}
