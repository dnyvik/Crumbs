using Crumbs.Core.Event;
using Newtonsoft.Json;
using System;

namespace Crumbs.Serializers.Json
{
    public class JsonEventSerializer : IEventSerializer
    {
        public string Serialize(IDomainEvent domainEvent)
        {
            return JsonConvert.SerializeObject(domainEvent);
        }

        public T Deserialize<T>(string data) where T : IDomainEvent
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public IDomainEvent Deserialize(string data, Type type)
        {
            return (IDomainEvent)JsonConvert.DeserializeObject(data, type);
        }
    }
}
