using Crumbs.Core.Session;
using Crumbs.EFCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Crumbs.EFCore.Session
{
    public abstract class SessionScopedContext : DbContext, IScopedContex
    {
        public SessionScopedContext(DbContextOptions options)
         : base(options) { }

        public void ScopeTo(IDataStoreScope scope)
        {
            Database.UseTransaction(scope.AsTransaction());
        }
    }
}

