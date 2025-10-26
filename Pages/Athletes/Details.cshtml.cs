using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OluRankings.Data;
using OluRankings.Models;

namespace OluRankings.Pages.Athletes
{
    public class AthleteDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public AthleteDetailsModel(ApplicationDbContext db) => _db = db;

        public Athlete? Athlete { get; set; }

        public async Task<IActionResult> OnGetAsync(string slug)
        {
            Athlete = await _db.Athletes
                .Include(a => a.StatLines)
                .FirstOrDefaultAsync(a => a.Slug == slug);

            return Athlete is null ? NotFound() : Page();
        }
    }
}
