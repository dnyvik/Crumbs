using System.Threading;
using System.Threading.Tasks;

namespace Crumbs.Core.Event
{
    /// <summary>
    /// For eventually consistency. Needs a better name!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHistoricalEventHandler<T> where T : IDomainEvent
    {
        Task Handle(T domainEvent, CancellationToken ct = default);
    }
}