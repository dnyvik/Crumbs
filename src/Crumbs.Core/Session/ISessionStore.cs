using System.Threading.Tasks;
using Crumbs.Core.Command;

namespace Crumbs.Core.Session
{
    public interface ISessionStore
    {
        Task Save(ICommand command);
    }
}