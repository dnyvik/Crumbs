using Crumbs.Core.Event.EventualConsistency;
using Crumbs.EFCore.Models;
using Crumbs.EFCore.Session;
using Crumbs.EventualConsistency;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Crumbs.EFCore.EventualConsistency
{
    public class EventHandlerStateStore : IEventHandlerStateStore
    {
        private readonly IFrameworkContextFactory _frameworkContextFactory;
        private readonly IEventHandlerStateSerializer _eventHandlerStateSerializer;

        public EventHandlerStateStore(
            IFrameworkContextFactory frameworkContextFactory,
            IEventHandlerStateSerializer eventHandlerStateSerializer)
        {
            _frameworkContextFactory = frameworkContextFactory;
            _eventHandlerStateSerializer = eventHandlerStateSerializer;
        }

        public async Task Delete(Guid stateId)
        {
            using (var context = await _frameworkContextFactory.CreateContext())
            {
                var state = context.EventHandlerStates.SingleOrDefault(s => s.Id == stateId);

                if (state != null)
                {
                    context.EventHandlerStates.Remove(state);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<StateContainer<T>> Get<T>(Guid stateId) where T : IEventHandlerState
        {
            using (var context = await _frameworkContextFactory.CreateContext())
            {
                var state = context.EventHandlerStates
                    .AsNoTracking()
                    .SingleOrDefault(s => s.Id == stateId);

                if (state != null)
                {
                    var typedState = _eventHandlerStateSerializer.Deserialize<T>(state.Data);

                    return new StateContainer<T>
                    {
                        Id = state.Id,
                        ProcessedEventId = state.ProcessedEventId,
                        LastUpdated = state.LastUpdated,
                        State = typedState,
                    };
                }

                return null;
            }
        }

        // Todo: Clean up
        public async Task Save<T>(StateContainer<T> stateContainer) where T : IEventHandlerState
        {
            using (var context = await _frameworkContextFactory.CreateContext())
            {
                var state = context.EventHandlerStates.SingleOrDefault(s => s.Id == stateContainer.Id);
                var insert = false;

                if (state == null)
                {
                    state = new EventHandlerState();
                    insert = true;
                }

                state.Id = stateContainer.Id;
                state.ProcessedEventId = stateContainer.ProcessedEventId;
                state.LastUpdated = stateContainer.LastUpdated;
                state.Status = stateContainer.HandlerStatus;
                state.Data = _eventHandlerStateSerializer.Serialize(stateContainer.State);

                if (insert)
                {
                    context.EventHandlerStates.Add(state);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
