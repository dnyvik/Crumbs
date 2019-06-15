using Crumbs.EFCore;
using Crumbs.EFCore.Session;
using Microsoft.EntityFrameworkCore;

namespace Crumbs.EFCore.ProviderContexts
{
    internal class SqliteDbContext : FrameworkContext
    {
        public SqliteDbContext() { }

        public SqliteDbContext(DbContextOptions<FrameworkContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlite(DataStoreConnectionFactory.ConnectionString);
            }
        }
    }

    // Migration command (from root project directory):
    //dotnet ef migrations add [MigrationName] --context SqliteDbContext --output-dir Migrations/SqliteMigrations --project src\Crumbs.EFCore\Crumbs.EFCore.csproj
}
