using Crumbs.Core.Snapshot;
using Newtonsoft.Json;
using System;

namespace Crumbs.Serializers.Json
{
    public class JsonSnapshotSerializer : ISnapshotSerializer
    {
        public string Serialize(Snapshot snapshot)
        {
            return JsonConvert.SerializeObject(snapshot);
        }

        public T Deserialize<T>(string data) where T : Snapshot
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public Snapshot Deserialize(string data, Type type)
        {
            return (Snapshot)JsonConvert.DeserializeObject(data, type);
        }
    }
}
