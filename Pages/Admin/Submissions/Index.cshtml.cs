using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OluRankings.Data;
using OluRankings.Models;
using OluRankings.Identity;

namespace OluRankings.Pages.Admin.Submissions   // <-- IMPORTANT
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Ranker}")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public IndexModel(ApplicationDbContext db) { _db = db; }

        public List<AthleteSubmission> Pending { get; set; } = new();

        public async Task OnGetAsync()
        {
            Pending = await _db.AthleteSubmissions
                .Where(s => s.Status == SubmissionStatus.Pending)
                .OrderBy(s => s.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
