﻿using System.Data.Common;

namespace Crumbs.EFCore.Session
{
    // Todo: Dispose db context from here?
    public class DatabaseTransactionWrapper : IDataStoreTransaction
    {
        public DbTransaction Transaction { get; }

        public DatabaseTransactionWrapper(DbTransaction transaction)
        {
            Transaction = transaction;
        }

        public void Commit()
        {
            Transaction.Commit();
        }

        public void Rollback()
        {
            Transaction.Rollback();
        }

        public void Dispose()
        {
            Transaction.Dispose();
        }
    }
}