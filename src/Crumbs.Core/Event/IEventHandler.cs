using System.Threading;
using System.Threading.Tasks;

namespace Crumbs.Core.Event
{
    public interface IEventHandler<T> where T : IDomainEvent
    {
        Task Handle(T domainEvent, CancellationToken ct = default);
    }
}