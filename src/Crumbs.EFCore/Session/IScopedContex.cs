using Crumbs.Core.Session;

namespace Crumbs.EFCore.Session
{
    public interface IScopedContex
    {
        void ScopeTo(IDataStoreScope scope);
    }
}

