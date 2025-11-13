using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OluRankings.Data;
using OluRankings.Models;

namespace OluRankings.Pages.Athletes
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public DetailsModel(ApplicationDbContext db) => _db = db;

        public Athlete? Item { get; private set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Item = await _db.Athletes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return Item is null ? NotFound() : Page();
        }
    }
}
