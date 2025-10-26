using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OluRankings.Data;
using OluRankings.Models;

namespace OluRankings.Pages.Rankings
{
    public class RankingsIndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public RankingsIndexModel(ApplicationDbContext db) => _db = db;

        [BindProperty(SupportsGet = true)] public string Publisher { get; set; } = "OluRankings";
        [BindProperty(SupportsGet = true)] public string Sport { get; set; } = "basketball";
        [BindProperty(SupportsGet = true)] public string AgeGroup { get; set; } = "U16";
        [BindProperty(SupportsGet = true)] public string? Period { get; set; }

        public List<string> Publishers { get; set; } = new();
        public List<string> Periods { get; set; } = new();

        public Ranking? Ranking { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Publishers = await _db.Rankings
                .Select(r => r.Publisher)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            Periods = await _db.Rankings
                .Where(r => r.Publisher == Publisher && r.Sport == Sport && r.AgeGroup == AgeGroup)
                .Select(r => r.Period)
                .Distinct()
                .OrderByDescending(x => x)
                .ToListAsync();

            var q = _db.Rankings
                .Include(r => r.Entries).ThenInclude(e => e.Athlete)
                .Where(r => r.Publisher == Publisher && r.Sport == Sport && r.AgeGroup == AgeGroup);

            if (!string.IsNullOrWhiteSpace(Period))
                q = q.Where(r => r.Period == Period);

            Ranking = await q
                .OrderByDescending(r => r.PublishedAt)
                .FirstOrDefaultAsync();

            return Page();
        }
    }
}
