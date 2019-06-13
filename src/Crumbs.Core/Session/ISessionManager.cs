using System;
using System.Threading;
using System.Threading.Tasks;
using Crumbs.Core.Aggregate;
using Crumbs.Core.Command;
using Crumbs.Core.Repositories;

namespace Crumbs.Core.Session
{
    public interface ISessionManager
    {
        Task<ISession> CreateSession(Guid sessionKey);
        Task<T> LoadAggregate<T>(Guid id, CancellationToken ct) where T : class, IAggregateRoot;
        Task Commit(ISession session, ICommand command);
        Task<IDataStoreScope> GetScopeForSession(Guid sessionKey, CancellationToken ct = default);
        void Initialize(
            IDataStoreConnectionFactory dataStoreConnectionFactory,
            IAggregateRootRepository repository,
            ISessionTracker sessionTracker);

        Task Rollback(ISession session);
    }
}