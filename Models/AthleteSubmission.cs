using System.ComponentModel.DataAnnotations;

namespace OluRankings.Models
{
    public enum SubmissionStatus { Pending = 0, Approved = 1, Rejected = 2 }

    public class AthleteSubmission
    {
        public int Id { get; set; }

        // Who submitted (IdentityUserId)
        [Required] public string SubmittedByUserId { get; set; } = default!;

        // The athlete details being verified (text captured at submission time)
        [Required, MaxLength(80)]  public string GivenName { get; set; } = default!;
        [Required, MaxLength(80)]  public string FamilyName { get; set; } = default!;
        [MaxLength(120)]           public string? School { get; set; }
        [MaxLength(120)]           public string? Club { get; set; }
        public int? ClassYear { get; set; }
        public DateOnly? Dob { get; set; }

        // Evidence (passport image/pdf) saved under wwwroot/evidence or Blob later
        [Required, MaxLength(512)] public string EvidencePath { get; set; } = default!;
        [MaxLength(64)]            public string? EvidenceContentType { get; set; }
        public long EvidenceBytes { get; set; }

        public SubmissionStatus Status { get; set; } = SubmissionStatus.Pending;
        [MaxLength(256)] public string? ReviewerNote { get; set; }
        public string? ReviewedByUserId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ReviewedAt { get; set; }

        // Once approved we can link it to the canonical Athlete record
        public Guid? AthleteId { get; set; }
        public Athlete? Athlete { get; set; }
    }
}
