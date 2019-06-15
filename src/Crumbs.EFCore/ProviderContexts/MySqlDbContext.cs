using Crumbs.EFCore;
using Crumbs.EFCore.Session;
using Microsoft.EntityFrameworkCore;

namespace Crumbs.EFCore.ProviderContexts
{
    internal class MySqlDbContext : FrameworkContext
    {
        public MySqlDbContext() { }

        public MySqlDbContext(DbContextOptions<FrameworkContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseMySql(DataStoreConnectionFactory.ConnectionString);
            }
        }
    }

    // Migration command (from root project directory):
    //dotnet ef migrations add [MigrationName] --context MySqlDbContext --output-dir Migrations/MySqlMigrations --project src\Crumbs.EFCore\Crumbs.EFCore.csproj
}
