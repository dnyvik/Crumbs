using System;

namespace Crumbs.Core.DependencyInjection
{
    public interface IResolver
    {
        T Resolve<T>() where T : class;
        object Resolve(Type type);
    }
}