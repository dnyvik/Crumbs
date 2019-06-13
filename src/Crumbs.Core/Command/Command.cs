using System;
using System.Runtime.Serialization;

namespace Crumbs.Core.Command
{
    public abstract class Command : ICommand
    {
        protected Command(Guid userId)
        {
            UserId = userId;
            Id = Guid.NewGuid();
        }

        // Used to determine if command has been applied before.
        [IgnoreDataMember]
        public Guid Id { get; }

        // All data mutations should have a user context to reflect who did what.
        public Guid UserId { get; }

        // For serializing sequence and if we want to log it to domain events.
        // More applicable for batch processing of commands. Revise at later point.
        //public DateTimeOffset ClientTimeStamp { get; set; }
    }
}