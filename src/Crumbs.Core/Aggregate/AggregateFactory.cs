using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Crumbs.Core.DependencyInjection;
using Crumbs.Core.Extensions;
using Crumbs.Core.Snapshot;

namespace Crumbs.Core.Aggregate
{
    public class AggregateFactory
    {
        private static IResolver _resolver;
        private static AggregateFactory _instance;
        private static Dictionary<Type, Type> _snapshotToTypeMap;

        public static AggregateFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    CreateInstance();
                }

                return _instance;
            }
        }

        private static void CreateInstance()
        {
            if (_resolver == null)
            {
                throw new MissingFieldException("Resolver needs to be set before a factory instance can be created.");
            }

            _instance = new AggregateFactory();
        }

        public static void Initialize(IResolver resolver, bool buildTypeMap = true)
        {
            _resolver = resolver;

            if (buildTypeMap)
            {
                _snapshotToTypeMap = BuildSnapshotToTypeMap();
            }
        }

        public T CreateAggregate<T>() where T : class, IAggregateRoot
        {
            return _resolver.Resolve<T>();
        }

        /// <summary>
        /// Use this method when you are trying to load a base type, which in it self does not implement
        /// the <see cref="ISnapshottable{T}" /> interface, but  you have a snapshot indicating
        /// that you need to instantiate child type instead.
        /// </summary>
        /// <typeparam name="T">The aggregate type</typeparam>
        /// <param name="snapshot">The snapshot</param>
        /// <returns>A subclass instance as <typeparam name="T"></typeparam></returns>
        public T CreateAggregate<T>(Snapshot.Snapshot snapshot) where T : class, IAggregateRoot
        {
            return (T)_resolver.Resolve(_snapshotToTypeMap[snapshot.GetType()]);
        }

        private static Dictionary<Type, Type> BuildSnapshotToTypeMap()
        {
            var snapshotToTypeMap = new Dictionary<Type, Type>();

            var snapshottableAggregateTypes = Assembly.GetAssembly(typeof(AggregateFactory))
                                                      .GetTypes()
                                                      .Where(t => t.IsClass && !t.IsAbstract && t.ImplementsGenericInterface(typeof(ISnapshottable<>)));

            foreach (var aggregateType in snapshottableAggregateTypes)
            {
                var snapshottableInterfaceType = aggregateType.GetInterfaces()
                    .First(t => t.IsConstructedGenericType && t.GetGenericTypeDefinition() == typeof(ISnapshottable<>));

                var snapshotTypeArgument = snapshottableInterfaceType
                    .GetGenericArguments()
                    .First(t => t.IsSubclassOf(typeof(Snapshot.Snapshot)));

                snapshotToTypeMap.Add(snapshotTypeArgument, aggregateType);
            }

            return snapshotToTypeMap;
        }
    }
}