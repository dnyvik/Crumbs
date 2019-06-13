using System;
using System.Threading.Tasks;
using Crumbs.Core.Aggregate;

namespace Crumbs.Core.Repositories
{
    // Todo: Async postfix
    public interface IAggregateRootRepository
    {
        Task Save<T>(T aggregate, Guid userId, Guid? sessionKey = null) where T : IAggregateRoot;
        Task<T> Get<T>(Guid aggregateId) where T : class, IAggregateRoot;
        Task Reload(Guid aggregateId);
        Task Delete(Guid aggregateId);
        Task TryMakeSnapshot(IAggregateRoot aggregate, Guid? scope = null);
    }
}