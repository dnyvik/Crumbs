using Crumbs.Core.Command;
using Crumbs.Core.Configuration;
using Crumbs.Core.Event;
using Crumbs.Core.Snapshot;

namespace Crumbs.Serializers.Json
{
    public static class FrameworkConfiguratorExtensions
    {
        public static FrameworkConfigurator UseJsonSerializers(this FrameworkConfigurator configurator)
        {
            configurator.RegisterSingelton<ICommandSerializer, JsonCommandSerializer>();
            configurator.RegisterSingelton<IEventSerializer, JsonEventSerializer>();
            configurator.RegisterSingelton<ISnapshotSerializer, JsonSnapshotSerializer>();

            return configurator;
        }

    }
}
