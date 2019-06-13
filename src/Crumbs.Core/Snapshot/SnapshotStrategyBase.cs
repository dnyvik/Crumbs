using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Crumbs.Core.Aggregate;

namespace Crumbs.Core.Snapshot
{
    public abstract class SnapshotStrategyBase
    {
        private readonly Dictionary<Type, bool> _knownTypes = new Dictionary<Type, bool>();

        public bool IsSnapshotable(Type aggregateType)
        {
            if (_knownTypes.ContainsKey(aggregateType))
            {
                return _knownTypes[aggregateType];
            }

            if (aggregateType.GetTypeInfo().BaseType == null)
            {
                _knownTypes.Add(aggregateType, false);
                return false;
            }

            if (ImplementsSnapshottableInterface(aggregateType))
            {
                _knownTypes.Add(aggregateType, true);
                return true;
            }

            return IsSnapshotable(aggregateType.GetTypeInfo().BaseType);
        }

        private static bool ImplementsSnapshottableInterface(Type aggregateType)
        {
            return aggregateType.GetTypeInfo().ImplementedInterfaces.Any(i => i.IsGenericType && i.GetTypeInfo().GetGenericTypeDefinition() == typeof(ISnapshottable<>));
        }
    }
}