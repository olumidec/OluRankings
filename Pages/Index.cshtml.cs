using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OluRankings.Data;
using OluRankings.Models;

namespace OluRankings.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public List<Ranking> LatestRankings { get; set; } = new();

        public IndexModel(ApplicationDbContext db) => _db = db;

        public async Task OnGetAsync()
        {
            LatestRankings = await _db.Rankings
                .OrderByDescending(r => r.PublishedAt)
                .Take(3)
                .ToListAsync();
        }
    }
}
