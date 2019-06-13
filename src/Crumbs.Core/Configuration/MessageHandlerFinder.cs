using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Crumbs.Core.Command;
using Crumbs.Core.Event;

namespace Crumbs.Core.Configuration
{
    public static class MessageToHandlerTypeFinder
    {
        public static IEnumerable<ValueTuple<Type, Type>> GetHandlerToMessageMappingTypes(List<Assembly> assemblies)
        {
            var allTypes = assemblies.SelectMany(a => a.GetTypes()).ToList();

            return GetCommandToHandlerMappingTypes(allTypes)
                .Concat(GetEventToHandlerMappingTypes(allTypes));
        }

        // Todo: Refactor type select into local methods
        private static IEnumerable<ValueTuple<Type, Type>> GetCommandToHandlerMappingTypes(List<Type> allTypes)
        {
            bool IsCommandHandlerInterface(Type t) => t.IsGenericType &&
                                                      t.GetGenericTypeDefinition() == typeof(ICommandHandler<>);

            bool IsCommandHandlerType(Type t) => t.IsClass &&
                                                 !t.IsGenericType &&
                                                 !t.IsAbstract &&
                                                 t.GetInterfaces().Any(IsCommandHandlerInterface);

            var commandHandlerTypes = allTypes.Where(IsCommandHandlerType);

            foreach (var commandHandlerType in commandHandlerTypes)
            {
                var commandType = commandHandlerType.GetInterfaces()
                    .Single(IsCommandHandlerInterface)
                    .GetGenericArguments()[0];

                yield return (commandType, commandHandlerType);
            }
        }

        private static IEnumerable<ValueTuple<Type, Type>> GetEventToHandlerMappingTypes(List<Type> allTypes)
        {
            bool IsEventHandlerInterface(Type t) => t.IsGenericType &&
                                                      t.GetGenericTypeDefinition() == typeof(IEventHandler<>);

            bool IsEventHandlerType(Type t) => t.IsClass &&
                                                 !t.IsGenericType &&
                                                 !t.IsAbstract &&
                                                 t.GetInterfaces().Any(IsEventHandlerInterface);

            var eventHandlerTypes = allTypes.Where(IsEventHandlerType);

            foreach (var eventHandlerType in eventHandlerTypes)
            {
                var eventHandlerDefinitions = eventHandlerType.GetInterfaces().Where(IsEventHandlerInterface);

                foreach (var eventHandlerDefinition in eventHandlerDefinitions)
                {
                    var eventType = eventHandlerDefinition.GetGenericArguments()[0];
                    yield return (eventType, eventHandlerType);
                }
            }
        }
    }
}