using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Crumbs.Core.Command;

namespace Crumbs.Core.Session
{
    public class SessionTracker : ISessionTracker
    {
        private readonly ISessionStore _sessionStore;

        // Todo: Wil grow for ever. Do we need this?
        private readonly HashSet<Guid> _completedSessions = new HashSet<Guid>();
        private readonly AsyncLock _completedSessionsMutex = new AsyncLock();

        //Could use this to run maintenance jobs that we don't want to keep track of (not taken into account as of yet, but kept as a reminder)
        public static readonly Guid MaintenenceSession = Guid.Parse("5d77efe6-09fb-46b1-9bcd-a457fabf6de4");

        public SessionTracker(ISessionStore sessionStore)
        {
            _sessionStore = sessionStore;
        }

        public async Task RollbackSession(Guid sessionKey)
        {
            using (await _completedSessionsMutex.LockAsync())
            {
                _completedSessions.Remove(sessionKey);
            }
        }

        public async Task<bool> HasCompleted(Guid sessionKey)
        {
            using (await _completedSessionsMutex.LockAsync())
            {
                return _completedSessions.Contains(sessionKey);
            }
        }

        public async Task CommitSession(ICommand command)
        {
            await _sessionStore.Save(command);

            using (await _completedSessionsMutex.LockAsync())
            {
                _completedSessions.Add(command.Id);
            }
        }
    }
}