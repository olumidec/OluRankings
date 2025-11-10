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

        // query string: ?q=&page=&pagesize=
        [BindProperty(SupportsGet = true)] public string? q { get; set; }
        [BindProperty(SupportsGet = true)] public int page { get; set; } = 1;
        [BindProperty(SupportsGet = true)] public int pagesize { get; set; } = 24;

        public int Total { get; private set; }
        public List<Athlete> Athletes { get; private set; } = new();

        public async Task OnGetAsync()
        {
            if (page < 1) page = 1;
            if (pagesize is < 6 or > 60) pagesize = 24;

            IQueryable<Athlete> query = _db.Athletes
                .AsNoTracking()
                .Where(a => a.Status == "active" && a.IsPublic);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(a =>
                    (a.GivenName + " " + a.FamilyName).ToLower().Contains(term) ||
                    (a.School ?? "").ToLower().Contains(term) ||
                    (a.Region ?? "").ToLower().Contains(term));
            }

            Total = await query.CountAsync();

            Athletes = await query
                .OrderBy(a => a.FamilyName).ThenBy(a => a.GivenName)
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToListAsync();
        }
    }
}
