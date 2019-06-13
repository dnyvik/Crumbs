using System;
using System.Threading.Tasks;

namespace Crumbs.History
{
    public class DefaultUserDescriptor : IUserDescriptor
    {
        public Task<string> GetUserDescription(Guid userId)
        {
            return Task.FromResult(userId.ToString());
        }
    }
}
