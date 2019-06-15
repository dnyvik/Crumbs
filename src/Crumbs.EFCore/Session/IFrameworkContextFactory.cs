using System;
using System.Threading.Tasks;

namespace Crumbs.EFCore.Session
{
    public interface IFrameworkContextFactory
    {
        Task<IFrameworkContext> CreateContext(Guid? session = null);
    }
}