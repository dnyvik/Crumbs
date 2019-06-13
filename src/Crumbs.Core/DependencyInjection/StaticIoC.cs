using System;
using System.Collections.Generic;
using System.Linq;

namespace Crumbs.Core.DependencyInjection
{
    // For testing. Todo: Move to test project?
    public class StaticIoC : IDependencyInjection
    {
        private Dictionary<Type, Func<object>> _typeFactoryMap = new Dictionary<Type, Func<object>>();
        private Dictionary<Type, object> _singeltons = new Dictionary<Type, object>();

        public T Resolve<T>() where T : class
        {
            if (!_typeFactoryMap.ContainsKey(typeof(T)))
            {
                throw new InvalidOperationException($"'{typeof(T)}' not registered.");
            }

            return (T)_typeFactoryMap[typeof(T)]();
        }

        public object Resolve(Type type)
        {
            if (!_typeFactoryMap.ContainsKey(type))
            {
                throw new InvalidOperationException($"'{type}' not registered.");
            }

            return _typeFactoryMap[type]();
        }

        public void RegisterSingelton<TInterface, TImplementation>()
             where TInterface : class
             where TImplementation : class, TInterface
        {
            _typeFactoryMap.Add(typeof(TInterface), () =>
            {
                var typeImplementation = typeof(TImplementation);

                if (!_singeltons.ContainsKey(typeImplementation))
                {
                    _singeltons.Add(typeImplementation, Activate(typeImplementation, _typeFactoryMap));
                }
                return _singeltons[typeImplementation];
            });
        }

        public void RegisterSingelton<TInterface>(TInterface instance)
             where TInterface : class
        {
            _typeFactoryMap.Add(typeof(TInterface), () =>
            {
                var typeImplementation = instance.GetType();

                if (!_singeltons.ContainsKey(typeImplementation))
                {
                    _singeltons.Add(typeImplementation, instance);
                }
                return _singeltons[typeImplementation];
            });
        }

        public void RegisterTransient(Type type)
        {
            _typeFactoryMap.Add(type, () => Activate(type, _typeFactoryMap));
        }

        public void RegisterTransient<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : class, TInterface
        {
            _typeFactoryMap.Add(typeof(TInterface), () => Activate(typeof(TImplementation), _typeFactoryMap));
        }

        private static object Activate(Type implementationType, Dictionary<Type, Func<object>> knownTypes)
        {
            var constructorParameterTypes = implementationType.GetConstructors()
                .Single()
                .GetParameters()
                .Select(pi => pi.ParameterType)
                .ToList();

            var unknownTypes = constructorParameterTypes.Where(t => !knownTypes.ContainsKey(t));
            if (unknownTypes.Any())
            {
                throw new InvalidOperationException($"Unknown types: '{string.Join(",", unknownTypes)}'");
            }

            var parameters = knownTypes.Where(t => constructorParameterTypes.Contains(t.Key))
                .Select(t => t.Value())
                .ToArray();

            return Activator.CreateInstance(implementationType, parameters);
        }
    }
}