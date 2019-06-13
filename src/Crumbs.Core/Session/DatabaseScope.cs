using System;

namespace Crumbs.Core.Session
{
    public class DatabaseScope : IDataStoreScope
    {
        public DatabaseScope(IDataStoreConnection connection, IDataStoreTransaction transaction)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        public IDataStoreConnection Connection { get; }

        public IDataStoreTransaction Transaction { get; }
    }
}