using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Crumbs.Core.Command
{
    public interface ICommandBatchProcessor
    {
        Task<IEnumerable<ICommandReponse>> Process(IEnumerable<ICommand> commands, CancellationToken ct = default);
    }
}
