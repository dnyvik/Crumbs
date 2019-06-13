using Crumbs.Core.Mediation;
using System;

namespace Crumbs.Core.Command
{
    public interface ICommand : IMessage
    {
        Guid Id { get; }
        Guid UserId { get; }

        // Might be nice to have for batch processing of commands.
        // DateTimeOffset ClientTimeStamp { get; }
    }
}