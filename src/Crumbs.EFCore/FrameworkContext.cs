using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Crumbs.EFCore.Extensions;
using Crumbs.EFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace Crumbs.EFCore
{
    public class FrameworkContext : DbContext, IFrameworkContext
    {
        public FrameworkContext() { }
        public FrameworkContext(DbContextOptions<FrameworkContext> options)
            : base(options) { }

        public DbSet<Models.Event> Events { get; set; }
        public DbSet<EventHandlerState> EventHandlerStates { get; set; }
        public DbSet<Snapshot> Snapshots { get; set; }
        public DbSet<Models.Session> Sessions { get; set; }

        public async Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return await Database.BeginTransactionAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("Framework");
            modelBuilder.SingularizeNames();

            modelBuilder.Entity<Models.Event>()
                .HasIndex(e => e.AggregateId);
        }

        public async Task<DbConnection> OpenDbConnection()
        {
            var connection = Database.GetDbConnection();
            await connection.OpenAsync();
            return connection;
        }

        public async Task MigrateAsync()
        {
            await Database.MigrateAsync();
        }

        public void Migrate()
        {
            Database.Migrate();
        }

        public IFrameworkContext UseTransaction(DbTransaction transaction)
        {
            Database.UseTransaction(transaction);
            return this;
        }
    }
}
