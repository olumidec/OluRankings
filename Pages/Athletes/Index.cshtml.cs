using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OluRankings.Data;
using OluRankings.Models;

namespace OluRankings.Pages.Athletes
{
    public class AthletesIndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public AthletesIndexModel(ApplicationDbContext db) => _db = db;

        // used by the Razor view: ?query=term
        [BindProperty(SupportsGet = true)]
        public string? Query { get; set; }

        public List<Athlete> Athletes { get; private set; } = new();

        public async Task OnGetAsync()
        {
            IQueryable<Athlete> q = _db.Athletes.AsNoTracking();

            // show only public profiles if you want that behaviour live:
            q = q.Where(a => a.IsPublic);

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
                .Take(60)
                .ToListAsync();
        }
    }
}
