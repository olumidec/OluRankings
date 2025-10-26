using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace OluRankings.Data
{
    /// <summary>
    /// Design-time factory so "dotnet ef" can construct ApplicationDbContext.
    /// Prefers AppDb (SQLite) in Development, falls back to Postgres if provided.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Always resolve config from the project directory (not /bin when EF runs)
            var basePath = Directory.GetCurrentDirectory();

            var cfg = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Prefer local SQLite during dev; Postgres for prod-like scenarios
            var sqlite   = cfg.GetConnectionString("AppDb");
            var postgres = cfg.GetConnectionString("Postgres");

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            if (!string.IsNullOrWhiteSpace(sqlite))
            {
                builder.UseSqlite(sqlite);
            }
            else if (!string.IsNullOrWhiteSpace(postgres))
            {
                builder.UseNpgsql(postgres);
            }
            else
            {
                // Safe default for dev tooling
                builder.UseSqlite("Data Source=olurankings_app.db");
            }

            return new ApplicationDbContext(builder.Options);
        }
    }
}
