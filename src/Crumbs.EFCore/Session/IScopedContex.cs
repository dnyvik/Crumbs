using Crumbs.Core.Session;
using System;

namespace Crumbs.EFCore.Session
{
    public interface IScopedContex : IDisposable
    {
        void ScopeTo(IDataStoreScope scope);
    }
}

