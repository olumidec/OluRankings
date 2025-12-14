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

        // Filters
        [BindProperty(SupportsGet = true)] public string Publisher { get; set; } = "OluRankings";
        [BindProperty(SupportsGet = true)] public string Sport { get; set; } = "basketball";
        [BindProperty(SupportsGet = true)] public string AgeGroup { get; set; } = "U16";
        [BindProperty(SupportsGet = true)] public string? Period { get; set; }

        // Sorting & paging
        [BindProperty(SupportsGet = true)] public string Sort { get; set; } = "rank"; // rank|name|team|region|score
        [BindProperty(SupportsGet = true)] public int PageNumber { get; set; } = 1;
        [BindProperty(SupportsGet = true)] public int PageSize { get; set; } = 25;

        // Dropdowns
        public List<string> Publishers { get; set; } = new();
        public List<string> Periods { get; set; } = new();

        // Selected ranking + rows
        public Ranking? Ranking { get; set; }
        public int TotalRows { get; set; }
        public IReadOnlyList<RankingEntry> Rows { get; set; } = Array.Empty<RankingEntry>();

        public int TotalPages => PageSize <= 0 ? 1 : (int)Math.Ceiling(TotalRows / (double)PageSize);

        public async Task<IActionResult> OnGetAsync()
        {
            if (PageNumber < 1) PageNumber = 1;
            if (PageSize is < 10 or > 100) PageSize = 25;
            Sort = (Sort ?? "rank").Trim().ToLowerInvariant();

            // Dropdowns
            Publishers = await _db.Rankings
                .AsNoTracking()
                .Select(r => r.Publisher)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            Periods = await _db.Rankings
                .AsNoTracking()
                .Where(r => r.Publisher == Publisher && r.Sport == Sport && r.AgeGroup == AgeGroup)
                .Select(r => r.Period)
                .Distinct()
                .OrderByDescending(x => x)
                .ToListAsync();

            // Pick the ranking (latest by PublishedAt, fallback CreatedAt)
            var rq = _db.Rankings
                .AsNoTracking()
                .Where(r => r.Publisher == Publisher && r.Sport == Sport && r.AgeGroup == AgeGroup);

            if (!string.IsNullOrWhiteSpace(Period))
                rq = rq.Where(r => r.Period == Period);

            Ranking = await rq
                .OrderByDescending(r => r.PublishedAt)
                .FirstOrDefaultAsync();

            if (Ranking is null)
            {
                TotalRows = 0;
                Rows = Array.Empty<RankingEntry>();
                return Page();
            }

            // Entries query (paged)
            IQueryable<RankingEntry> eq = _db.RankingEntries
                .AsNoTracking()
                .Where(e => e.RankingId == Ranking.Id)
                .Include(e => e.Athlete);

            // Sorting
            eq = Sort switch
            {
                "name" => eq.OrderBy(e => e.Athlete.FamilyName).ThenBy(e => e.Athlete.GivenName),
                "team" => eq.OrderBy(e => e.Athlete.School).ThenBy(e => e.Athlete.Club).ThenBy(e => e.Rank),
                "region" => eq.OrderBy(e => e.Athlete.Region).ThenBy(e => e.Rank),
                "score" => eq.OrderByDescending(e => e.Score).ThenBy(e => e.Rank),
                _ => eq.OrderBy(e => e.Rank)
            };

            TotalRows = await eq.CountAsync();

            // Clamp page number if it’s beyond the end
            var tp = TotalPages;
            if (tp > 0 && PageNumber > tp) PageNumber = tp;

            Rows = await eq
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page();
        }
    }
}
