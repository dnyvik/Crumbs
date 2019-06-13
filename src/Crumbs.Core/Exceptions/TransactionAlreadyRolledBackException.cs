using System;

namespace Crumbs.Core.Exceptions
{
    public class TransactionAlreadyRolledBackException : Exception
    {
        public TransactionAlreadyRolledBackException()
           : base("Transaction has already been rolled back.") { }
    }
}