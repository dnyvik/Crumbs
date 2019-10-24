using Crumbs.Core.DependencyInjection;
using System;

namespace Crumbs.DependencyInjection.ServiceCollection
{
    public class ServiceProviderWrapper : IDependencyInjection
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderWrapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void RegisterSingelton<TInterface>(TInterface instance) where TInterface : class
        {
            throw new InvalidOperationException("Cannot register dependencies after registration process is done.");
        }

        public void RegisterTransient(Type type)
        {
            throw new InvalidOperationException("Cannot register dependencies after registration process is done.");
        }

        public T Resolve<T>() where T : class
        {
            return (T)_serviceProvider.GetService(typeof(T));
        }

        public object Resolve(Type type)
        {
            return _serviceProvider.GetService(type);
        }

        void IDependencyRegistry.RegisterSingelton<TInterface, TImplementation>()
        {
            throw new InvalidOperationException("Cannot register dependencies after registration process is done.");
        }

        void IDependencyRegistry.RegisterTransient<TInterface, TImplementation>()
        {
            throw new InvalidOperationException("Cannot register dependencies after registration process is done.");
        }
    }
}
