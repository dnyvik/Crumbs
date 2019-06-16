using System;
using Crumbs.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Crumbs.DependencyInjection.ServiceCollection
{
    public class ServiceCollectionWrapper : IDependencyInjection
    {
        private readonly IServiceCollection _services;
        private IServiceProvider _provider;

        public ServiceCollectionWrapper(IServiceCollection services)
        {
            _services = services;
        }

        private IServiceProvider Provider
        {
            get
            {
                if (_provider == null)
                {
                    _provider = _services.BuildServiceProvider();
                }

                return _provider;
            }
        }

        public void RegisterSingelton<TInterface>(TInterface instance)
            where TInterface : class
        {
            _services.AddSingleton(instance);
        }

        public void RegisterTransient(Type type)
        {
            _services.AddTransient(type);
        }

        public void RegisterTransient<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : class, TInterface
        {
            _services.AddTransient<TInterface, TImplementation>();
        }

        public T Resolve<T>() where T : class
        {
            return Provider.GetRequiredService<T>();
        }

        public object Resolve(Type type)
        {
            return Provider.GetRequiredService(type);
        }

        public void RegisterSingelton<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : class, TInterface
        {
            _services.AddSingleton<TInterface, TImplementation>();
        }
    }
}
