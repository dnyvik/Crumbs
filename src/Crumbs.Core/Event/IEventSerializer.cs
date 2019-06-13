using System;

namespace Crumbs.Core.Event
{
    public interface IEventSerializer
    {
        string Serialize(IDomainEvent domainEvent);
        T Deserialize<T>(string data) where  T : IDomainEvent;
        IDomainEvent Deserialize(string data, Type type);
    }
}