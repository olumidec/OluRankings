namespace OluRankings.Models;

public class Ranking
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Who published this list (lets you host OluRankings, RichardElite, UK50, etc.)
    public string Publisher { get; set; } = "OluRankings";

    public string Sport { get; set; } = "basketball";
    public string AgeGroup { get; set; } = "U16";
    public string? Position { get; set; }
    public string? Region { get; set; }
    public string Period { get; set; } = "2025-Q4";
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<RankingEntry> Entries { get; set; } = new List<RankingEntry>();
}

