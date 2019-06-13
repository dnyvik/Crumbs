using System.Threading;
using System.Threading.Tasks;

namespace Crumbs.Core.Event
{
    public interface IEventPublisher
    {
        Task Publish<T>(T domainEvent, CancellationToken ct = default) where T : IDomainEvent;
    }
}