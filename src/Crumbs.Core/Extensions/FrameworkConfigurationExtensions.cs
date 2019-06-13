using Crumbs.Core.Configuration;

namespace Crumbs.Core.Extensions
{
    public static class FrameworkConfigurationExtensions
    {
        public static T GetValue<T>(this IFrameworkConfiguration configuration, string key)
        {
            return (T)configuration.Values[key];
        }
    }
}