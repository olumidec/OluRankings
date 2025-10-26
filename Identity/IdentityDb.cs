using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OluRankings.Models;

namespace OluRankings.Identity
{
    public class IdentityDb : IdentityDbContext<ApplicationUser>
    {
        // MUST be generic: DbContextOptions<IdentityDb>
        public IdentityDb(DbContextOptions<IdentityDb> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
