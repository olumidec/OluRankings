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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public EditModel(ApplicationDbContext db) => _db = db;

        [BindProperty(SupportsGet = true)] public Guid Id { get; set; }

        public Ranking? List { get; set; }
        public List<RankingEntry> Entries { get; set; } = new();
        public List<Athlete> AllAthletes { get; set; } = new();

        [BindProperty(SupportsGet = true)] public string? Q { get; set; }
        [BindProperty] public Guid[] OrderedAthleteIds { get; set; } = Array.Empty<Guid>();

        public async Task<IActionResult> OnGetAsync()
        {
            List = await _db.Rankings
                .Include(r => r.Entries).ThenInclude(e => e.Athlete)
                .FirstOrDefaultAsync(r => r.Id == Id);

            if (List is null) return NotFound();

            Entries = List.Entries.OrderBy(e => e.Rank).ToList();

            IQueryable<Athlete> aQ = _db.Athletes.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(Q))
            {
                var term = Q.Trim().ToLower();
                aQ = aQ.Where(a => (a.GivenName + " " + a.FamilyName).ToLower().Contains(term)
                                 || (a.School ?? "").ToLower().Contains(term)
                                 || (a.Region ?? "").ToLower().Contains(term));
            }
            AllAthletes = await aQ.OrderBy(a => a.FamilyName).ThenBy(a => a.GivenName).Take(50).ToListAsync();
            return Page();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostSaveOrderAsync(Guid id)
        {
            var list = await _db.Rankings.Include(r => r.Entries).FirstOrDefaultAsync(r => r.Id == id);
            if (list is null) return NotFound();

            _db.RankingEntries.RemoveRange(list.Entries);
            list.Entries = OrderedAthleteIds
                .Select((athId, idx) => new RankingEntry
                {
                    RankingId = id,
                    AthleteId = athId,
                    Rank = idx + 1
                }).ToList();

            await _db.SaveChangesAsync();
            TempData["Msg"] = "Order saved.";
            return RedirectToPage(new { id });
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAddAsync(Guid id, Guid athleteId)
        {
            var exists = await _db.RankingEntries.AnyAsync(e => e.RankingId == id && e.AthleteId == athleteId);
            if (!exists)
            {
                var maxRank = await _db.RankingEntries.Where(e => e.RankingId == id).Select(e => (int?)e.Rank).MaxAsync() ?? 0;
                _db.RankingEntries.Add(new RankingEntry { RankingId = id, AthleteId = athleteId, Rank = maxRank + 1 });
                await _db.SaveChangesAsync();
            }
            return RedirectToPage(new { id });
        }
    }
}
