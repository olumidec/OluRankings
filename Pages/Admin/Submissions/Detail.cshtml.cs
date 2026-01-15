using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OluRankings.Data;
using OluRankings.Models;
using OluRankings.Identity;

namespace OluRankings.Pages.Admin.Submissions
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Ranker}")]
    public class DetailModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public DetailModel(ApplicationDbContext db) { _db = db; }

        public AthleteSubmission? Submission { get; set; }

        [BindProperty] public bool CreateNewAthlete { get; set; } = true;
        [BindProperty] public Guid? ExistingAthleteId { get; set; }

        [BindProperty] public string? RejectReason { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Submission = await _db.AthleteSubmissions.FindAsync(id);
            return Submission is null ? NotFound() : Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var s = await _db.AthleteSubmissions.FirstOrDefaultAsync(x => x.Id == id);
            if (s is null || s.Status != SubmissionStatus.Pending) return NotFound();

            Athlete athlete;
            if (CreateNewAthlete || ExistingAthleteId is null)
            {
                athlete = new Athlete {
                    GivenName = s.GivenName,
                    FamilyName = s.FamilyName,
                    School = s.School,
                    Club = s.Club,
                    ClassYear = s.ClassYear,
                    Slug = $"{s.GivenName}-{s.FamilyName}-{Guid.NewGuid().ToString("N")[..6]}".ToLowerInvariant()
                };
                _db.Athletes.Add(athlete);
            }
            else
            {
                athlete = await _db.Athletes.FindAsync(ExistingAthleteId.Value)
                          ?? throw new Exception("Athlete not found");
            }

            s.Status = SubmissionStatus.Approved;

            // ✅ DB expects text (string) right now
            s.ReviewedAt = DateTime.UtcNow.ToString("O");
            s.ReviewedByUserId = User.Identity?.Name;

            s.Athlete = athlete;

            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            var s = await _db.AthleteSubmissions.FirstOrDefaultAsync(x => x.Id == id);
            if (s is null || s.Status != SubmissionStatus.Pending) return NotFound();

            s.Status = SubmissionStatus.Rejected;
            s.ReviewerNote = string.IsNullOrWhiteSpace(RejectReason) ? null : RejectReason.Trim();

            // ✅ DB expects text (string) right now
            s.ReviewedAt = DateTime.UtcNow.ToString("O");
            s.ReviewedByUserId = User.Identity?.Name;

            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
