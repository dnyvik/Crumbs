using Crumbs.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crumbs.EventualConsistency.Extensions
{
    public static class FrameworkConfiguratorExtensions
    {
        public static FrameworkConfigurator UseEventualConsistancyHandlers(this FrameworkConfigurator configurator)
        {
            configurator.RegisterSingelton<IEventStreamer, EventStreamer>();

            return configurator;
        }

        // EventHandlerProcessor

        // Event handler type i schema til crumbs

        // -> Hent alle handlers som har historical interface. Grupper på type.
        // Les all state -> Initialiser de som har state basert på serialisert type. Lag state for de andre og spool.
        // Config for hvor hissig man skal hente events
        // Retry på event.
        // Kontrol actions for start stop osv?


        // TODO 2: Lage konsept på toppen av live stream? Events som kun er av interesse i øyeblikket (type chat),
        // men som ikke er kritisk at lagres ol. Livestream + filter på event handlers. Error handling hvis man ønsker (pokemon bool?)
        // Må vel egentlig ha dette for at vi ikke skal drepe live streamen. Fire and forget handler.


        // Attributt på event som gjør at vi kan deserialisere det til noe annet (overdide for full name)
    }
}
