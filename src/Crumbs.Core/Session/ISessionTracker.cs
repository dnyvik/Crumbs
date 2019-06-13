using System;
using System.Threading.Tasks;
using Crumbs.Core.Command;

namespace Crumbs.Core.Session
{
    // Todo: Do we really need this anymore? Every provider should handle duplicate keys.
    public interface ISessionTracker
    {
        Task CommitSession(ICommand command);
        Task RollbackSession(Guid sessionKey);
        Task<bool> HasCompleted(Guid sessionKey);
    }
}