using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Crumbs.Core.Aggregate;
using Crumbs.Core.Command;
using Crumbs.Core.DependencyInjection;
using Crumbs.Core.Event;
using Crumbs.Core.Exceptions;
using Crumbs.Core.Mediation;
using Crumbs.Core.Repositories;
using Crumbs.Core.Session;
using Crumbs.Core.Snapshot;

namespace Crumbs.Core.Configuration
{
    public class FrameworkConfigurator
    {
        private readonly Action<FrameworkConfigurationValues> _completeAction;
        private readonly FrameworkConfigurationValues _configuration;
        private IDependencyInjection _ioc;
        private List<Action> _registerActions = new List<Action>();
        private List<Assembly> _assembliesToScan = new List<Assembly>();
        private List<ValueTuple<Type, Type>> _handlerToMessageMapping = new List<(Type, Type)>();

        // Todo: To Tasks
        private List<Action<IResolver>> _initializationActions = new List<Action<IResolver>>();

        internal FrameworkConfigurator(Action<FrameworkConfigurationValues> completeAction)
        {
            _completeAction = completeAction;
            _configuration = new FrameworkConfigurationValues();
        }

        public FrameworkConfigurator RegisterTransientType<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : class, TInterface
        {
            _registerActions.Add(() => _ioc.RegisterTransient<TInterface, TImplementation>());
            return this;
        }

        public FrameworkConfigurator RegisterSingelton<TInterface>(TInterface instance)
            where TInterface : class
        {
            _registerActions.Add(() => _ioc.RegisterSingelton(instance));
            return this;
        }

        public FrameworkConfigurator RegisterSingelton<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : class, TInterface
        {
            _registerActions.Add(() => _ioc.RegisterSingelton<TInterface, TImplementation>());
            return this;
        }

        public FrameworkConfigurator SetDependencyFramework(IDependencyInjection dependencyFramwork)
        {
            _ioc = dependencyFramwork;
            return this;
        }

        public FrameworkConfigurator UseHandlersFrom(params Assembly[] assemblies)
        {
            _assembliesToScan.AddRange(assemblies);
            return this;
        }

        public FrameworkConfigurator AddConfigurationValue(string key, object value)
        {
            _configuration.AddValue(key, value);
            return this;
        }

        public FrameworkConfigurator RegisterInitializationAction(Action<IResolver> action)
        {
            _initializationActions.Add(action);
            return this;
        }

        //Mediation
        public FrameworkConfigurator UseInproccessMediation()
        {
            var bus = new InProcessBus(_ioc);

            _ioc.RegisterSingelton<ICommandSender>(bus);
            _ioc.RegisterSingelton<IEventPublisher>(bus);
            _ioc.RegisterSingelton<IMessageHandlerRegistry>(bus);
            _ioc.RegisterSingelton<IEventRelay>(bus);

            return this;
        }

        public void Run()
        {
            ScanForHandlersAndMessages();
            Validate();
            RegisterDependencies();
            RegisterInternalInitializationActions();
            RunInitializeActions();

            _completeAction(_configuration);
        }

        private void RegisterInternalInitializationActions()
        {
            RegisterInitializationAction((resolver) =>
            {
                var connectionFactory = resolver.Resolve<IDataStoreConnectionFactory>();
                var repository = resolver.Resolve<IAggregateRootRepository>();
                var sessionTracker = resolver.Resolve<ISessionTracker>();
                _ioc.Resolve<ISessionManager>().Initialize(connectionFactory, repository, sessionTracker);
            });

            RegisterInitializationAction((resolver) =>
            {
                // Register in ioc instead and inject?
                AggregateFactory.Initialize(resolver);
            });
        }

        // For testing (remove when testing is done)
        public IResolver TestRun()
        {
            Run();
            return _ioc;
        }

        private void RunInitializeActions()
        {
            foreach (var action in _initializationActions)
            {
                action(_ioc);
            }
        }

        private void ScanForHandlersAndMessages()
        {
            var coreAssembly = Assembly.GetAssembly(typeof(ICommandHandler<>));

            if (!_assembliesToScan.Contains(coreAssembly))
            {
                _assembliesToScan.Add(coreAssembly);
            }

            _handlerToMessageMapping.AddRange(MessageToHandlerTypeFinder.GetHandlerToMessageMappingTypes(_assembliesToScan));
        }

        public void Validate()
        {
            if (_ioc == null)
            {
                throw new FrameworkConfigurationException("No dependency injection framework has been set.");
            }

            if (!_handlerToMessageMapping.Any())
            {
                throw new FrameworkConfigurationException("No handlers registered.");
            }
        }

        private void RegisterDependencies()
        {
            RegisterInternal();
            RegisterExternal();
            RegisterAggregates();
            RegisterMessageHandlers();
        }

        private void RegisterAggregates()
        {
            foreach(var aggregateType in AggregateRootFinder.GetAllTypes(_assembliesToScan))
            {
                _ioc.RegisterTransient(aggregateType);
            }
        }

        private void RegisterInternal()
        {
            _ioc.RegisterSingelton<IFrameworkConfiguration>(_configuration);
            _ioc.RegisterSingelton<IResolver>(_ioc);
            _ioc.RegisterSingelton<IAggregateRootRepository, AggregateRootRepository>();
            _ioc.RegisterSingelton<ISessionTracker, SessionTracker>();
            _ioc.RegisterSingelton<ISnapshotRestoreUtility, SnapshotRestoreUtility>();
            _ioc.RegisterSingelton<ISessionManager, SessionManager>();
        }

        private void RegisterExternal()
        {
            foreach (var initializationAction in _registerActions)
            {
                initializationAction();
            }
        }

        private void RegisterMessageHandlers()
        {
            // Todo: cleanup

            var handlerMapping = new List<(Type, Type)>();

            var handlerGroupings = _handlerToMessageMapping.GroupBy(v => v.Item2);

            // We need to register handlers before we build resolver
            foreach (var handlerGrouping in handlerGroupings)
            {
                _ioc.RegisterTransient(handlerGrouping.Key);
            }

            // First call to resolver might build it. Dependencies used by framework needs to be registered before this.
            var registry = _ioc.Resolve<IMessageHandlerRegistry>();

            foreach (var handlerGrouping in handlerGroupings)
            {
                foreach (var (messageType, handlerType) in handlerGrouping)
                {
                    registry.RegisterHandler(messageType, handlerType);
                }
            }
        }
    }
}
