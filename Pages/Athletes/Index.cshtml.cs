using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OluRankings.Data;          // ApplicationDbContext
using OluRankings.Models;        // Athlete

namespace OluRankings.Pages.Athletes
{
    public class AthletesIndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public AthletesIndexModel(ApplicationDbContext db) => _db = db;

        [BindProperty(SupportsGet = true)]
        public string? Query { get; set; }

        public List<Athlete> Athletes { get; private set; } = new();

        public async Task OnGetAsync()
        {
            // Base query
            IQueryable<Athlete> q = _db.Athletes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(Query))
            {
                var term = Query.Trim().ToLower();
                q = q.Where(a =>
                    (a.GivenName + " " + a.FamilyName).ToLower().Contains(term) ||
                    (a.School ?? "").ToLower().Contains(term) ||
                    (a.Region ?? "").ToLower().Contains(term));
            }

            Athletes = await q
                .OrderBy(a => a.FamilyName).ThenBy(a => a.GivenName)
                .Take(60)                    // keep the page snappy
                .ToListAsync();
        }
    }
}
