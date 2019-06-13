using System;

namespace Crumbs.Core.Aggregate
{
    // Todo: Try out
    // Currently not in use (we cache the snapshots and load these), but it might be a better idea to cache the aggregates instead)?
    // See implementation example underneath.
    public interface IAggregateCache
    {
        bool IsTracked(Guid id);
        void Set(Guid id, AggregateRoot aggregate);
        AggregateRoot Get(Guid id);
        void Remove(Guid id);
    }

    //public class AggregateCache : IAggregateCache
    //{
    //    private readonly MemoryCache _cache;
    //    private readonly Func<CacheItemPolicy> _policyFactory;

    //    public AggregateCache()
    //    {
    //        _cache = MemoryCache.Default;
    //        _policyFactory = () => new CacheItemPolicy();
    //    }

    //    public void Set(Guid id, AggregateRoot aggregate)
    //    {
    //        _cache.Set(id.ToString(), aggregate, _policyFactory.Invoke());
    //    }

    //    public AggregateRoot Get(Guid id)
    //    {
    //        return (AggregateRoot)_cache.Get(id.ToString());
    //    }

    //    public void Remove(Guid id)
    //    {
    //        _cache.Remove(id.ToString());
    //    }

    //    public bool IsTracked(Guid id)
    //    {
    //        return _cache.Contains(id.ToString());
    //    }
    //}

}