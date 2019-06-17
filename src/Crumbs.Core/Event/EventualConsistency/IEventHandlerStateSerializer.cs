namespace Crumbs.Core.Event.EventualConsistency
{
    public interface IEventHandlerStateSerializer
    {
        string Serialize(IEventHandlerState eventHandlerState);
        T Deserialize<T>(string data) where T : IEventHandlerState;
    }
}
