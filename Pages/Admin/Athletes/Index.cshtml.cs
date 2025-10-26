using Microsoft.AspNetCore.Authorization;
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

        public async Task OnGetAsync(string? q)
        {
            var baseQ = _db.Athletes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                baseQ = baseQ.Where(a =>
                    a.GivenName.ToLower().Contains(term) ||
                    a.FamilyName.ToLower().Contains(term) ||
                    (a.School ?? "").ToLower().Contains(term) ||
                    (a.Club ?? "").ToLower().Contains(term));
            }

            Items = await baseQ
                .OrderBy(a => a.FamilyName)
                .ThenBy(a => a.GivenName)
                .Take(200)
                .ToListAsync();
        }
    }
}
