using System;

namespace Crumbs.Core.Exceptions
{
    public class TransactionAlreadyCommittedException : Exception
    {
        public TransactionAlreadyCommittedException()
           : base("Transaction has already been committed.") { }
    }
}