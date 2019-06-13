using System;

namespace Crumbs.Core.Aggregate
{
    public class Entity : IEntity
    {
        public Guid Id { get; set; }

        Guid IEntity.Id => Id;
    }
}
