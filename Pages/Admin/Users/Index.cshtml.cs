using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OluRankings.Identity;
using OluRankings.Models; // <-- this is what fixes it

namespace OluRankings.Pages.Admin.Users
{
    [Authorize(Roles = $"{AppRoles.Admin}")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _um;
        public IndexModel(UserManager<ApplicationUser> um) => _um = um;

        public record Row(string Id, string? Email, IList<string> Roles);
        public List<Row> Users { get; set; } = new();

        public async Task OnGetAsync()
        {
            foreach (var u in _um.Users.ToList())
            {
                var r = await _um.GetRolesAsync(u);
                Users.Add(new Row(u.Id, u.Email, r));
            }
        }
    }
}
