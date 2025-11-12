using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OluRankings.Data;
using OluRankings.Identity;
using OluRankings.Models;

namespace OluRankings.Pages.Admin.Rankings
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Ranker}")]
    public class CreateModel : PageModel
    {
         private readonly ApplicationDbContext _db;
    public CreateModel(ApplicationDbContext db) => _db = db;

    [BindProperty] public Ranking NewRanking { get; set; } = new();

   public void OnGet()
{
    NewRanking.Publisher = "OluRankings";
    NewRanking.Sport     = "basketball";
    NewRanking.AgeGroup  = "U16";
    NewRanking.Period    = "2025-Q4";
    NewRanking.PublishedAt = DateTime.UtcNow; // <-- fixes CS0037
}


    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        NewRanking.Id        = Guid.NewGuid();
        NewRanking.CreatedAt = DateTime.UtcNow;

        _db.Rankings.Add(NewRanking);
        await _db.SaveChangesAsync();
        return RedirectToPage("Edit", new { id = NewRanking.Id });
        }
    }
}
