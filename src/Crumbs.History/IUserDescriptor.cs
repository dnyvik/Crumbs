using System;
using System.Threading.Tasks;

namespace Crumbs.History
{
    public interface IUserDescriptor
    {
        Task<string> GetUserDescription(Guid userId);
    }
}
