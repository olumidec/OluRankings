using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OluRankings.Data;
using OluRankings.Identity;
using OluRankings.Models;

namespace OluRankings.Pages.Admin.Rankings // <-- IMPORTANT
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Ranker}")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public EditModel(ApplicationDbContext db) { _db = db; }

        public Ranking? List { get; set; }
        public List<Athlete> AllAthletes { get; set; } = new();

        [BindProperty] public Guid[] OrderedAthleteIds { get; set; } = Array.Empty<Guid>();

        public async Task<IActionResult> OnGetAsync(Guid id)          // <-- Guid
        {
            List = await _db.Rankings
                .Include(r => r.Entries)
                .ThenInclude(e => e.Athlete)
                .FirstOrDefaultAsync(r => r.Id == id);                // Guid == Guid

            if (List is null) return NotFound();

            AllAthletes = await _db.Athletes
                .OrderBy(x => x.FamilyName)
                .ThenBy(x => x.GivenName)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostSaveOrderAsync(Guid id) // <-- Guid
        {
            var list = await _db.Rankings
                .Include(r => r.Entries)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (list is null) return NotFound();

            _db.RankingEntries.RemoveRange(list.Entries);
            list.Entries = OrderedAthleteIds
                .Select((athId, idx) => new RankingEntry
                {
                    AthleteId = athId,
                    RankingId = id,                                   // Guid -> Guid
                    Rank = idx + 1
                })
                .ToList();

            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
