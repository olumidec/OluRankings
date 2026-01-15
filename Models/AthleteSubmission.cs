using System;
using System.ComponentModel.DataAnnotations;

namespace OluRankings.Models
{
    public enum SubmissionStatus { Pending = 0, Approved = 1, Rejected = 2 }

    /// <summary>
    /// Matches Postgres table: "AthleteSubmissions"
    /// (current DB types: CreatedAt=text, ReviewedAt=text, EvidenceBytes=integer, Dob=date)
    /// </summary>
    public class AthleteSubmission
    {
        // Id: integer NOT NULL (now IDENTITY in Postgres)
        public int Id { get; set; }

        // SubmittedByUserId: text NOT NULL
        [Required]
        public string SubmittedByUserId { get; set; } = default!;

        // GivenName: text NOT NULL
        [Required, MaxLength(80)]
        public string GivenName { get; set; } = default!;

        // FamilyName: text NOT NULL
        [Required, MaxLength(80)]
        public string FamilyName { get; set; } = default!;

        // School: text NULL
        [MaxLength(120)]
        public string? School { get; set; }

        // Club: text NULL
        [MaxLength(120)]
        public string? Club { get; set; }

        // ClassYear: integer NULL
        public int? ClassYear { get; set; }

        // Dob: date NULL
        public DateOnly? Dob { get; set; }

        // EvidencePath: text NOT NULL
        [Required, MaxLength(512)]
        public string EvidencePath { get; set; } = default!;

        // EvidenceContentType: text NULL
        [MaxLength(64)]
        public string? EvidenceContentType { get; set; }

        // EvidenceBytes: integer NOT NULL  ✅ (DB is integer, not bigint)
        public int EvidenceBytes { get; set; }

        // Status: integer NOT NULL
        public SubmissionStatus Status { get; set; } = SubmissionStatus.Pending;

        // ReviewerNote: text NULL
        [MaxLength(256)]
        public string? ReviewerNote { get; set; }

        // ReviewedByUserId: text NULL
        public string? ReviewedByUserId { get; set; }

        // CreatedAt: text NOT NULL  ✅ (DB is text right now)
        [Required]
        public string CreatedAt { get; set; } = default!;

        // ReviewedAt: text NULL  ✅ (DB is text right now)
        public string? ReviewedAt { get; set; }

        // AthleteId: uuid NULL
        public Guid? AthleteId { get; set; }
        public Athlete? Athlete { get; set; }

        // NOTE: If you add VideoUrl / Region / Position / AgeGroup to DB later,
        // add properties here too.
    }
}
