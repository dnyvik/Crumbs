using Crumbs.Core.Command;
using System.Threading;
using System.Threading.Tasks;

namespace Crumbs.Core.Session
{
    public abstract class SessionScopedCommandHandler<T> : ICommandHandler<T> where T : ICommand
    {
        protected readonly ISessionManager SessionManager;

        protected SessionScopedCommandHandler(ISessionManager sessionManager)
        {
            SessionManager = sessionManager;
        }

        public async Task Handle(T command, CancellationToken ct = default)
        {
            using (var session = await SessionManager.CreateSession(command.Id))
            {
                await Handle(command, session, ct);
                await session.Commit(command);
            }
        }

        public abstract Task Handle(T command, ISession session, CancellationToken ct);
    }
}