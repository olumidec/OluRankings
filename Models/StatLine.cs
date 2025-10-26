namespace OluRankings.Models;

public class StatLine
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AthleteId { get; set; }
    public Athlete Athlete { get; set; } = default!;

    public string Season { get; set; } = "2025";
    public string? Competition { get; set; }
    public int GamesPlayed { get; set; }
    public double PointsPerGame { get; set; }
    public double AssistsPerGame { get; set; }
    public double ReboundsPerGame { get; set; }

    public bool IsVerified { get; set; }
    public string? EvidenceUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
