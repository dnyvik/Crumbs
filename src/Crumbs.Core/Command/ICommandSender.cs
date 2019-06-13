using System.Threading;
using System.Threading.Tasks;

namespace Crumbs.Core.Command
{
    public interface ICommandSender
    {
        Task Send<T>(T command, CancellationToken ct = default) where T : ICommand;
    }
}