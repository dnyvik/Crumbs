using System;
using Crumbs.Core.Aggregate;

namespace Crumbs.Core.Repositories
{
    public interface IEntityRepository<T> where T : IEntity
    {
        void Save(T entity);
        T Get(Guid entityId);
    }
}