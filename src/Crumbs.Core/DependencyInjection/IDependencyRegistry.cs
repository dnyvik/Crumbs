using System;

namespace Crumbs.Core.DependencyInjection
{
    // Todo: Consider what we need here after testing is done 
    public interface IDependencyRegistry
    {
        void RegisterSingelton<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : class, TInterface;

        void RegisterSingelton<TInterface>(TInterface instance)
            where TInterface : class;

        void RegisterTransient(Type type);
        void RegisterTransient<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : class, TInterface;
    }
}