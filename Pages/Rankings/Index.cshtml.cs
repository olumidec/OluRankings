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

        // filters (existing)
        [BindProperty(SupportsGet = true)] public string Publisher { get; set; } = "OluRankings";
        [BindProperty(SupportsGet = true)] public string Sport { get; set; } = "basketball";
        [BindProperty(SupportsGet = true)] public string AgeGroup { get; set; } = "U16";
        [BindProperty(SupportsGet = true)] public string? Period { get; set; }

        // sorting & paging for entries
        [BindProperty(SupportsGet = true)] public string Sort { get; set; } = "rank"; // rank|name|team|region|score
        [BindProperty(SupportsGet = true)] public int Page { get; set; } = 1;
        [BindProperty(SupportsGet = true)] public int PageSize { get; set; } = 25;

        public List<string> Publishers { get; set; } = new();
        public List<string> Periods { get; set; } = new();

        public Ranking? Ranking { get; set; }
        public int TotalRows { get; set; }
        public IReadOnlyList<RankingEntry> Rows { get; set; } = Array.Empty<RankingEntry>();

        public async Task<IActionResult> OnGetAsync()
        {
            if (Page < 1) Page = 1;
            if (PageSize is < 10 or > 100) PageSize = 25;

            // dropdowns
            Publishers = await _db.Rankings
                .Select(r => r.Publisher).Distinct()
                .OrderBy(x => x).ToListAsync();

            Periods = await _db.Rankings
                .Where(r => r.Publisher == Publisher && r.Sport == Sport && r.AgeGroup == AgeGroup)
                .Select(r => r.Period).Distinct()
                .OrderByDescending(x => x).ToListAsync();

            // pick the ranking
            var rq = _db.Rankings
                .AsNoTracking()
                .Where(r => r.Publisher == Publisher && r.Sport == Sport && r.AgeGroup == AgeGroup);

            if (!string.IsNullOrWhiteSpace(Period))
                rq = rq.Where(r => r.Period == Period);

            Ranking = await rq.OrderByDescending(r => r.PublishedAt).FirstOrDefaultAsync();

            if (Ranking == null)
            {
                TotalRows = 0;
                Rows = Array.Empty<RankingEntry>();
                return Page();
            }

            // entries query kept as IQueryable<RankingEntry> (important for OrderBy)
            IQueryable<RankingEntry> eq = _db.RankingEntries
                .AsNoTracking()
                .Where(e => e.RankingId == Ranking.Id)
                .Include(e => e.Athlete);

            // sorting
            eq = Sort.ToLowerInvariant() switch
            {
                "name"   => eq.OrderBy(e => e.Athlete.FamilyName).ThenBy(e => e.Athlete.GivenName),
                "team"   => eq.OrderBy(e => e.Athlete.School).ThenBy(e => e.Athlete.Club),
                "region" => eq.OrderBy(e => e.Athlete.Region).ThenBy(e => e.Rank),
                "score"  => eq.OrderByDescending(e => e.Score).ThenBy(e => e.Rank),
                _        => eq.OrderBy(e => e.Rank) // default
            };

            TotalRows = await eq.CountAsync();
            Rows = await eq
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page();
        }
    }
}
