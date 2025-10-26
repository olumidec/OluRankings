using System.ComponentModel.DataAnnotations;

namespace OluRankings.Models;

public class RankingEntry
{
    // No Id: we’ll use a composite PK
    public Guid RankingId { get; set; }
    public Ranking Ranking { get; set; } = default!;

    public Guid AthleteId { get; set; }
    public Athlete Athlete { get; set; } = default!;

    public int Rank { get; set; }                 // 1..N within a Ranking
    public double Score { get; set; }
    public string? Notes { get; set; }
}
