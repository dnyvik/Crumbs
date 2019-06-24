using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Crumbs.Core.Aggregate;

namespace Crumbs.Core.Configuration
{
    public static class AggregateRootFinder
    {
        public static IEnumerable<Type> GetAllTypes(List<Assembly> assemblies)
        {
            return assemblies.SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(AggregateRoot)));
        }
    }
}