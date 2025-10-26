using Microsoft.AspNetCore.Authorization;
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
                .Include(r => r.Entries)
                .OrderByDescending(r => r.PublishedAt)
                .ToListAsync();
        }
    }
}
