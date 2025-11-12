using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OluRankings.Data;
using OluRankings.Identity;
using OluRankings.Models;

namespace OluRankings.Pages.Admin.Rankings
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Ranker}")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public IndexModel(ApplicationDbContext db) => _db = db;

        public List<Ranking> Items { get; set; } = new();

        public async Task OnGetAsync()
        {
            Items = await _db.Rankings
                .AsNoTracking()
                .OrderByDescending(r => r.PublishedAt)
                .ThenByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var r = await _db.Rankings.FirstOrDefaultAsync(x => x.Id == id);
            if (r != null)
            {
                _db.RankingEntries.RemoveRange(_db.RankingEntries.Where(e => e.RankingId == id));
                _db.Rankings.Remove(r);
                await _db.SaveChangesAsync();
                TempData["Msg"] = "Ranking deleted.";
            }
            return RedirectToPage();
        }
    }
}
