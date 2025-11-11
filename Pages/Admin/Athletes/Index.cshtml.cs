using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OluRankings.Data;
using OluRankings.Identity;
using OluRankings.Models;

namespace OluRankings.Pages.Admin.Athletes
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Ranker},{AppRoles.Coach}")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public IndexModel(ApplicationDbContext db) => _db = db;

        public List<Athlete> Items { get; set; } = new();

        // GET /Admin/Athletes?q=...
        public async Task OnGetAsync(string? q)
        {
            var baseQ = _db.Athletes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                baseQ = baseQ.Where(a =>
                    (a.GivenName + " " + a.FamilyName).ToLower().Contains(term) ||
                    (a.School ?? "").ToLower().Contains(term) ||
                    (a.Club ?? "").ToLower().Contains(term) ||
                    (a.Region ?? "").ToLower().Contains(term));
            }

            Items = await baseQ
                .OrderBy(a => a.FamilyName).ThenBy(a => a.GivenName)
                .Take(200)
                .ToListAsync();
        }

        // POST /Admin/Athletes?handler=TogglePublic
        public async Task<IActionResult> OnPostTogglePublicAsync(Guid id, bool value, string? q)
        {
            var a = await _db.Athletes.FirstOrDefaultAsync(x => x.Id == id);
            if (a == null) return NotFound();

            a.IsPublic = value;
            a.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return RedirectToPage(new { q });
        }

        // POST /Admin/Athletes?handler=SoftDelete
        public async Task<IActionResult> OnPostSoftDeleteAsync(Guid id, string? q)
        {
            var a = await _db.Athletes.FirstOrDefaultAsync(x => x.Id == id);
            if (a == null) return NotFound();

            // soft remove: hide from public listings, preserve record
            a.IsPublic = false;
            a.Status = "removed";
            a.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return RedirectToPage(new { q });
        }
    }
}
