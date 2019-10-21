using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crumbs.Core.Command;
using Crumbs.Core.DependencyInjection;
using Crumbs.Core.Event;

namespace Crumbs.Core.Mediation
{
    public class InProcessBus : ICommandSender, IEventPublisher, IMessageHandlerRegistry, IEventRelay
    {
        private readonly Dictionary<Type, List<Type>> _handlersMap
            = new Dictionary<Type, List<Type>>();

        private readonly List<Action<IDomainEvent>> _relays
            = new List<Action<IDomainEvent>>();

        private readonly IResolver _resolver;

        public InProcessBus(IResolver resolver)
        {
            _resolver = resolver;
        }

        public void RegisterRelayHandler(Action<IDomainEvent> handler)
        {
            _relays.Add(handler);
        }

        public async Task Send<T>(T command, CancellationToken ct = default) where T : ICommand
        {
            var commandType = command.GetType();

            if (_handlersMap.TryGetValue(commandType, out List<Type> handlerTypes))
            {
                if (handlerTypes.Count != 1)
                {
                    throw new InvalidOperationException($"There are more than one handler for command '{commandType}'. " +
                        $"Handlers found: {string.Join(',', handlerTypes.Select(x => x.FullName))}. "  +
                        $"Framework only supports a single handler per command type at any given time.");
                }

                var handler = _resolver.Resolve(handlerTypes[0]) as ICommandHandler<T>;

                if (handler == null)
                {
                    throw new Exception($"Could not resolve command handler for command type '{commandType}'.");
                }

                await handler.Handle(command, ct);
            }
            else
            {
                throw new InvalidOperationException($"No handler registered for command type '{commandType}'.");
            }
        }

        public async Task Publish<T>(T domainEvent, CancellationToken ct = default) where T : IDomainEvent
        {
            var eventType = domainEvent.GetType();

            if (_handlersMap.TryGetValue(eventType, out List<Type> handlerTypes))
            {
                foreach (var handlerType in handlerTypes)
                {
                    var handlerInstance = _resolver.Resolve(handlerType);

                    if (handlerInstance == null)
                    {
                        throw new Exception($"Could not resolve event handler for event type '{eventType}'.");
                    }

                    var handler = handlerInstance as IEventHandler<T>;

                    if (handler != null)
                    {
                        await handler.Handle(domainEvent, ct);
                    }
                    else
                    {
                        // Todo: Fix
                        // For when IDomainEvent is the type parameter.
                        await (handlerInstance as dynamic).Handle((dynamic)domainEvent, ct);
                    }
                }
            }

            foreach (var relay in _relays)
            {
                relay(domainEvent);
            }
        }

        public void RegisterHandler(Type messageType, Type handlerType)
        {
            if (!_handlersMap.TryGetValue(messageType, out List<Type> handlers))
            {
                handlers = new List<Type>();
                _handlersMap.Add(messageType, handlers);
            }

            handlers.Add(handlerType);
        }
    }
}
