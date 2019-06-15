using Crumbs.Core.Session;
using Crumbs.EFCore.Session;
using System.Data.Common;

namespace Crumbs.EFCore.Extensions
{
    public static class DataStoreScopeExtensions
    {
        public static DbConnection AsDbConnection(this IDataStoreScope scope)
        {
            var connection = (DatabaseConnectionWrapper)scope.Connection;
            return connection.Connection;
        }

        public static DbTransaction AsTransaction(this IDataStoreScope scope)
        {
            var transaction = (DatabaseTransactionWrapper)scope.Transaction;
            return transaction.Transaction;
        }
    }
}