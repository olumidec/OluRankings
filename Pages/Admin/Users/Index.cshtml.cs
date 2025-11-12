using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
            string? DisplayName,
            bool EmailConfirmed,
            bool Locked,
            string[] Roles
        );

        public List<UserRow> Users { get; private set; } = new();

        [BindProperty(SupportsGet = true)] public string? Q { get; set; }

        [BindProperty, Required] public string UserId { get; set; } = string.Empty;
        [BindProperty] public string RoleName { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            IQueryable<ApplicationUser> q = _userManager.Users;

            if (!string.IsNullOrWhiteSpace(Q))
            {
                var term = Q.Trim().ToUpperInvariant();
                q = q.Where(u =>
                    (u.Email ?? "").ToUpper().Contains(term) ||
                    (u.UserName ?? "").ToUpper().Contains(term) ||
                    (u.DisplayName ?? "").ToUpper().Contains(term));
            }

            var list = await q.AsNoTracking().OrderBy(u => u.Email).ToListAsync();
            foreach (var u in list)
            {
                var roles = await _userManager.GetRolesAsync(u);
                Users.Add(new UserRow(
                    Id: u.Id,
                    Email: u.Email ?? u.UserName ?? "(no email)",
                    DisplayName: u.DisplayName,
                    EmailConfirmed: u.EmailConfirmed,
                    Locked: u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.UtcNow,
                    Roles: roles.OrderBy(r => r).ToArray()
                ));
            }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostConfirmEmailAsync()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null) return NotFound();

            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                TempData["Msg"] = $"Email confirmed for {user.Email}.";
            }
            return RedirectToPage(new { Q });
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostToggleLockAsync()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null) return NotFound();

            // Don’t allow locking the last Admin out
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                var otherAdmins = admins.Count(u => u.Id != user.Id);
                if (otherAdmins == 0)
                {
                    TempData["Err"] = "You are the last Admin. Add another Admin before locking this account.";
                    return RedirectToPage(new { Q });
                }
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
                user.LockoutEnd = null;
            else
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(10);
            }

            await _userManager.UpdateAsync(user);
            TempData["Msg"] = "Lock status updated.";
            return RedirectToPage(new { Q });
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAddRoleAsync()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(RoleName))
            {
                if (!await _roleManager.RoleExistsAsync(RoleName))
                    await _roleManager.CreateAsync(new IdentityRole(RoleName));

                if (!await _userManager.IsInRoleAsync(user, RoleName))
                {
                    await _userManager.AddToRoleAsync(user, RoleName);
                    TempData["Msg"] = $"Added role {RoleName} to {user.Email}.";
                }
            }
            return RedirectToPage(new { Q });
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostRemoveRoleAsync()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(RoleName))
            {
                // Guard: don’t remove Admin from last admin
                if (RoleName == "Admin")
                {
                    var admins = await _userManager.GetUsersInRoleAsync("Admin");
                    if (admins.Count == 1 && admins[0].Id == user.Id)
                    {
                        TempData["Err"] = "Cannot remove Admin: this is the last Admin.";
                        return RedirectToPage(new { Q });
                    }
                }

                if (await _userManager.IsInRoleAsync(user, RoleName))
                {
                    await _userManager.RemoveFromRoleAsync(user, RoleName);
                    TempData["Msg"] = $"Removed role {RoleName} from {user.Email}.";
                }
            }
            return RedirectToPage(new { Q });
        }
    }
}
