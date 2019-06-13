using System;
using System.Linq;
using System.Reflection;

namespace Crumbs.Core.Extensions
{
    public static class TypeExtensions
    {
        public static bool ImplementsGenericInterface(this Type actualType, Type genericInterfaceType)
        {
            return actualType.GetTypeInfo().ImplementedInterfaces
                             .Any(i => i.IsConstructedGenericType
                                       && i.GetGenericTypeDefinition() == genericInterfaceType);
        }
    }
}
