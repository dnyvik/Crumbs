using Microsoft.EntityFrameworkCore;

namespace Crumbs.EFCore.ProviderContexts
{
    internal class SqliteDbContext : FrameworkContext
    {
        public SqliteDbContext(DbContextOptions<FrameworkContext> options)
            : base(options) { }
    }

    // Migration command (from root project directory):
    //dotnet ef migrations add [MigrationName] --context SqliteDbContext --output-dir Migrations/SqliteMigrations --project src\Crumbs.EFCore\Crumbs.EFCore.csproj
}
