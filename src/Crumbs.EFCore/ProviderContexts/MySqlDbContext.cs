using Microsoft.EntityFrameworkCore;

namespace Crumbs.EFCore.ProviderContexts
{
    internal class MySqlDbContext : FrameworkContext
    {
        public MySqlDbContext(DbContextOptions<FrameworkContext> options)
            : base(options) { }
    }

    // Migration command (from root project directory):
    //dotnet ef migrations add [MigrationName] --context MySqlDbContext --output-dir Migrations/MySqlMigrations --project src\Crumbs.EFCore\Crumbs.EFCore.csproj
}
