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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public EditModel(ApplicationDbContext db) => _db = db;

        [BindProperty] public Athlete? Athlete { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Athlete = await _db.Athletes.FirstOrDefaultAsync(a => a.Id == id);
            return Athlete is null ? NotFound() : Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Athlete is null) return BadRequest();
            if (!ModelState.IsValid) return Page();

            var existing = await _db.Athletes.FirstOrDefaultAsync(a => a.Id == Athlete.Id);
            if (existing is null) return NotFound();

            // Map editable fields
            existing.GivenName = Athlete.GivenName;
            existing.FamilyName = Athlete.FamilyName;
            existing.Slug = Athlete.Slug;
            existing.Sport = Athlete.Sport;
            existing.Position = Athlete.Position;
            existing.ClassYear = Athlete.ClassYear;
            existing.School = Athlete.School;
            existing.Club = Athlete.Club;
            existing.Region = Athlete.Region;
            existing.HeightCm = Athlete.HeightCm;
            existing.WeightKg = Athlete.WeightKg;
            existing.HeadshotUrl = Athlete.HeadshotUrl;
            existing.Status = Athlete.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            TempData["Saved"] = true;
            return RedirectToPage("Index");
        }
    }
}
