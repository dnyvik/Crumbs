using System.Data;

namespace Crumbs.Core.Session
{
    public interface IDataStoreScope
    {
        IDataStoreConnection Connection { get; }
        IDataStoreTransaction Transaction { get; }
    }
}