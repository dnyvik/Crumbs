using System;
using System.Threading.Tasks;

public interface IDataStoreConnectionFactory
{
    Task<IDataStoreConnection> Connect();
}
