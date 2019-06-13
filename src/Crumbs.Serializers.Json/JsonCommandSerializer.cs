using Crumbs.Core.Command;
using Newtonsoft.Json;
using System;

namespace Crumbs.Serializers.Json
{
    public class JsonCommandSerializer : ICommandSerializer
    {
        public string Serialize(ICommand command)
        {
            return JsonConvert.SerializeObject(command);
        }

        public T Deserialize<T>(string data) where T : ICommand
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public ICommand Deserialize(string data, Type type)
        {
            return (ICommand)JsonConvert.DeserializeObject(data, type);
        }
    }
}
