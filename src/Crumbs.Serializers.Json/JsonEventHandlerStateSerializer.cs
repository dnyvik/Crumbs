using Crumbs.Core.Event.EventualConsistency;
using Newtonsoft.Json;

namespace Crumbs.Serializers.Json
{
    public class JsonEventHandlerStateSerializer : IEventHandlerStateSerializer
    {
        public T Deserialize<T>(string data) where T : IEventHandlerState
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public string Serialize(IEventHandlerState eventHandlerState)
        {
            return JsonConvert.SerializeObject(eventHandlerState);
        }
    }
}
