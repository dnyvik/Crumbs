using System;

namespace Crumbs.Core.Command
{
    // Todo: Should we use this for regular handlers as well?
    public class CommandReponse : ICommandReponse
    {
        private CommandReponse(Guid commandId, string message, bool successfullyApplied)
        {
            CommandId = commandId;
            Message = message;
            SuccessfullyApplied = successfullyApplied;
        }
        public Guid CommandId { get; set; }
        public string Message { get; set; }
        public bool SuccessfullyApplied { get; }

        public static CommandReponse Success(ICommand command)
        {
            return new CommandReponse(command.Id, "Success", true);
        }

        public static CommandReponse Failed(ICommand command, Exception e)
        {
            return new CommandReponse(command.Id, e.Message, false);
        }
    }
}