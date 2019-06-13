using System.Threading;
using System.Threading.Tasks;

namespace Crumbs.Core.Command
{
    public interface ICommandHandler<T> where T : ICommand
    {
        Task Handle(T command, CancellationToken ct = default);
    }
}