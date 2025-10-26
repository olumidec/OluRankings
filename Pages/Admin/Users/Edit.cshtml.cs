using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OluRankings.Identity;
using OluRankings.Models; // <-- this is what fixes it

namespace OluRankings.Pages.Admin.Users
{
    [Authorize(Roles = $"{AppRoles.Admin}")]
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _um;
        private readonly RoleManager<IdentityRole> _rm;

        public EditModel(UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm)
        {
            _um = um; _rm = rm;
        }

        public ApplicationUser? UserEntity { get; set; }
        public List<string> AllRoles { get; set; } = AppRoles.All.ToList();

        [BindProperty] public string? Email { get; set; }
        [BindProperty] public string[] Roles { get; set; } = Array.Empty<string>();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _um.FindByIdAsync(id);
            if (user is null) return NotFound();

            UserEntity = user;
            Email = user.Email;
            Roles = (await _um.GetRolesAsync(user)).ToArray();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var u = await _um.FindByIdAsync(id);
            if (u is null) return NotFound();

            if (!string.Equals(u.Email, Email, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(Email))
            {
                u.Email = Email;
                u.UserName = Email;
                await _um.UpdateAsync(u);
            }

            var current = await _um.GetRolesAsync(u);
            await _um.RemoveFromRolesAsync(u, current);
            await _um.AddToRolesAsync(u, Roles);

            TempData["Saved"] = true;
            return RedirectToPage("Index");
        }
    }
}
