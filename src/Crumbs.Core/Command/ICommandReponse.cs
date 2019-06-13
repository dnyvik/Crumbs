using System;

namespace Crumbs.Core.Command
{
    public interface ICommandReponse
    {
        Guid CommandId { get; }
        string Message { get; }
        bool SuccessfullyApplied { get; }
    }
}