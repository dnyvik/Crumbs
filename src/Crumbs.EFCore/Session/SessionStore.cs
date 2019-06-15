using Crumbs.Core.Command;
using Crumbs.Core.Session;
using System;
using System.Threading.Tasks;

namespace Crumbs.EFCore.Session
{
    public class SessionStore : ISessionStore
    {
        private readonly ICommandSerializer _commandSerializer;
        private readonly IFrameworkContextFactory _frameworkContextFactory;

        public SessionStore(
            ICommandSerializer commandSerializer,
            IFrameworkContextFactory frameworkContextFactory)
        {
            _commandSerializer = commandSerializer;
            _frameworkContextFactory = frameworkContextFactory;
        }

        public async Task Save(ICommand command)
        {
            using (var context = await _frameworkContextFactory.CreateContext(command.Id))
            {
                context.Sessions.Add(new Models.Session
                {
                    Id = command.Id,
                    CompletedDate = DateTimeOffset.Now, //Todo: Time service?
                    ComittedByUserId = command.UserId,
                    Type = command.GetType().AssemblyQualifiedName,
                    Data = _commandSerializer.Serialize(command),
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
