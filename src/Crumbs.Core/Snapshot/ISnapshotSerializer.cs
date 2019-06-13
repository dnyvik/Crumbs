using System;

namespace Crumbs.Core.Snapshot
{
    public interface ISnapshotSerializer
    {
        string Serialize(Snapshot snapshot);
        T Deserialize<T>(string body) where  T : Snapshot;
        Snapshot Deserialize(string body, Type type);
    }
}