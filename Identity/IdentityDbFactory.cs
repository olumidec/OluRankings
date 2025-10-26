using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace OluRankings.Identity
{
    /// <summary>Used by "dotnet ef" to construct IdentityDb at design-time.</summary>
    public class IdentityDbFactory : IDesignTimeDbContextFactory<IdentityDb>
    {
        public IdentityDb CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            var cfg = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var cs = cfg.GetConnectionString("IdentityDb") ?? "Data Source=olurankings_identity.db";

            // GENERIC builder <IdentityDb> is crucial here
            var opts = new DbContextOptionsBuilder<IdentityDb>()
                .UseSqlite(cs)
                .Options;

            return new IdentityDb(opts);
        }
    }
}
