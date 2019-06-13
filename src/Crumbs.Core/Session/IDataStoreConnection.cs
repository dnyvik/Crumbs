using System;
using System.Data;

public interface IDataStoreConnection : IDisposable
{
    IDataStoreTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
}
