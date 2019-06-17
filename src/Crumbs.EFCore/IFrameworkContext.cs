using Crumbs.EFCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Crumbs.EFCore
{
    public interface IFrameworkContext : IDisposable
    {
        DbSet<EventHandlerState> EventHandlerStates { get; set; }
        DbSet<Models.Event> Events { get; set; }
        DbSet<Models.Session> Sessions { get; set; }
        DbSet<Snapshot> Snapshots { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task<System.Data.Common.DbConnection> OpenDbConnection();
        Task Migrate();
        IFrameworkContext UseTransaction(System.Data.Common.DbTransaction transaction);
    }
}