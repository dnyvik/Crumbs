using System;
using Microsoft.Extensions.DependencyInjection;

namespace Crumbs.DependencyInjection.ServiceCollection
{
    public class ServiceCollectionRegistryWrapper : ServiceCollectionWrapper
    {
        public ServiceCollectionRegistryWrapper(IServiceCollection services) 
            : base(services) {}

        public override object Resolve(Type type)
        {
            throw new InvalidOperationException("Cannot resolve dependencies during registration process.");
        }

        public override T Resolve<T>()
        {
            throw new InvalidOperationException("Cannot resolve dependencies during registration process.");
        }
    }
}
