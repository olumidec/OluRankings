using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OluRankings.Identity; // for ApplicationUser

namespace OluRankings.Models;

public class Athlete
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(450)] public string? UserId { get; set; } // link to account if they own this profile
    [ForeignKey(nameof(UserId))] public ApplicationUser? User { get; set; }

    [Required, MaxLength(80)]  public string GivenName { get; set; } = default!;
    [Required, MaxLength(80)]  public string FamilyName { get; set; } = default!;
    [Required, MaxLength(120)] public string Slug { get; set; } = default!;

    [MaxLength(40)] public string Sport { get; set; } = "basketball";
    [MaxLength(40)] public string? Position { get; set; }
    public int? ClassYear { get; set; }
    [MaxLength(120)] public string? School { get; set; }
    [MaxLength(120)] public string? Club { get; set; }
    [MaxLength(80)]  public string? Region { get; set; }
    public int? HeightCm { get; set; }
    public int? WeightKg { get; set; }
    public string? HeadshotUrl { get; set; }
    public string Status { get; set; } = "active";

    // Models/Athlete.cs (additions)
    public bool IsPublic { get; set; } = false;   // controls site visibility
    public bool IsVerified => VerifiedAt != null; // convenience computed


    // Verification + audit
    public DateTime? VerifiedAt { get; set; }
    [MaxLength(450)] public string? VerifiedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<StatLine> StatLines { get; set; } = new List<StatLine>();
}
