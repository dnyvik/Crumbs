using System;

public interface IDataStoreTransaction : IDisposable
{
    void Commit();
    void Rollback();
}
