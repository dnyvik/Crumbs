using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Crumbs.Core.Command;

namespace Crumbs.Core.Mediation
{
    public class CommandBatchProcessor : ICommandBatchProcessor
    {
        private readonly ICommandSender _commandSender;

        public CommandBatchProcessor(ICommandSender commandSender)
        {
            _commandSender = commandSender;
        }

        public async Task<IEnumerable<ICommandReponse>> Process(IEnumerable<ICommand> commands, CancellationToken ct = default)
        {
            var result = new List<CommandReponse>();

            foreach (var command in commands)
            {
                if (ct.IsCancellationRequested)
                {
                    return result;
                }

                try
                {
                    await _commandSender.Send(command);
                    result.Add(CommandReponse.Success(command));
                }
                catch (Exception e)
                {
                    result.Add(CommandReponse.Failed(command, e));
                }
            }

            return result;
        }
    }
}