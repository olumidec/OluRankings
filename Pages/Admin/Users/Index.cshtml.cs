using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OluRankings.Models;

namespace OluRankings.Pages.Admin.Users
{
    [Authorize(Policy = "RequireAdmin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IndexModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public record UserRow(
            string Id,
            string Email,
            bool EmailConfirmed,
            bool LockoutEnabled,
            string? LockoutEnd,
            string[] Roles
        );

        public List<UserRow> Users { get; private set; } = new();

        [BindProperty, Required]
        public string UserId { get; set; } = string.Empty;

        [BindProperty]
        public string RoleName { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            var users = _userManager.Users.ToList();
            Users = new List<UserRow>(users.Count);

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                Users.Add(new UserRow(
                    Id: u.Id,
                    Email: u.Email ?? u.UserName ?? "(no email)",
                    EmailConfirmed: u.EmailConfirmed,
                    LockoutEnabled: u.LockoutEnabled,
                    LockoutEnd: u.LockoutEnd?.ToString(),
                    Roles: roles.ToArray()
                ));
            }
        }

        public async Task<IActionResult> OnPostConfirmEmailAsync()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null) return NotFound();

            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostToggleLockAsync()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null) return NotFound();

            // Toggle: if locked (LockoutEnd in future) → clear it, else enable lock & set a future end
            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                user.LockoutEnd = null;
            }
            else
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(10);
            }

            await _userManager.UpdateAsync(user);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddRoleAsync()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(RoleName))
            {
                if (!await _roleManager.RoleExistsAsync(RoleName))
                    await _roleManager.CreateAsync(new IdentityRole(RoleName));

                if (!await _userManager.IsInRoleAsync(user, RoleName))
                    await _userManager.AddToRoleAsync(user, RoleName);
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveRoleAsync()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(RoleName))
            {
                if (await _userManager.IsInRoleAsync(user, RoleName))
                    await _userManager.RemoveFromRoleAsync(user, RoleName);
            }

            return RedirectToPage();
        }
    }
}
