using System;
using System.Data;
using Crumbs.Core.Exceptions;

// Todo: Should we use this anymore? EFCore transactions are automagically rolled back on dispose I think.
public abstract class Transaction : IDataStoreTransaction
{
    private readonly Action _action;
    private readonly Action _rollback;
    private bool _committed;
    private bool _rolledBack;

    public Transaction(
        Action action,
        Action rollback = null,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        _action = action;
        _rollback = rollback;
        IsolationLevel = isolationLevel;
    }

    // Todo: To async
    public void Commit()
    {
        if (_committed)
            throw new TransactionAlreadyCommittedException();

        try
        {
            _action();
            CommitImplementation();
        }
        catch (Exception)
        {
            Rollback();
            throw;
        }
        finally
        {
            _committed = true;
        }
    }

    // Todo: To async
    public void Rollback()
    {
        if (_rolledBack)
            throw new TransactionAlreadyRolledBackException();

        RollbackImplementation();
        _rollback?.Invoke();
    }

    protected IsolationLevel IsolationLevel { get; }

    // Todo: To async
    protected abstract void CommitImplementation();
    // Todo: To async
    protected abstract void RollbackImplementation();

    public void Dispose()
    {
        if (!_committed && !_rolledBack)
            Rollback();
    }
}
