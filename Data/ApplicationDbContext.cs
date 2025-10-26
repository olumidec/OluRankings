using Microsoft.EntityFrameworkCore;
using OluRankings.Models;

namespace OluRankings.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

    public DbSet<Athlete> Athletes => Set<Athlete>();
    public DbSet<StatLine> StatLines => Set<StatLine>();
    public DbSet<Ranking> Rankings => Set<Ranking>();
    public DbSet<RankingEntry> RankingEntries => Set<RankingEntry>();
    public DbSet<AthleteSubmission> AthleteSubmissions => Set<AthleteSubmission>();

     protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // Athlete
        b.Entity<Athlete>().HasIndex(a => a.Slug).IsUnique();
        b.Entity<Athlete>().HasIndex(a => new { a.FamilyName, a.GivenName });

        // RankingEntry composite PK
        b.Entity<RankingEntry>().HasKey(x => new { x.RankingId, x.AthleteId });

        // Unique ranking “identity”: one list per publisher/sport/period/scope
        b.Entity<Ranking>()
            .HasIndex(r => new { r.Publisher, r.Sport, r.AgeGroup, r.Position, r.Region, r.Period })
            .IsUnique();

        // Submissions: status filter fast
        b.Entity<AthleteSubmission>().HasIndex(s => s.Status);
    }
}
