using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Crumbs.Core.Extensions
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> GetAllImplementationsOfInterfaceType(this Assembly assembly, Type interfaceType)
        {
            bool HasInterface(Type t) => t == interfaceType || (t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType);

            bool ImplementsInterface(Type t) => t.IsClass &&
                                                 !t.IsGenericType &&
                                                 !t.IsAbstract &&
                                                 t.GetInterfaces().Any(HasInterface);

            return assembly.GetTypes().Where(ImplementsInterface);
        }
    }
}
