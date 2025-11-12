using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OluRankings.Data;
using OluRankings.Identity;
using OluRankings.Models;

namespace OluRankings.Pages.Admin.Submissions
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Ranker}")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public IndexModel(ApplicationDbContext db) => _db = db;

        public List<AthleteSubmission> Pending { get; private set; } = new();

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
