using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OluRankings.Identity;

namespace OluRankings.Pages.Admin
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Ranker}")]
    public class IndexModel : PageModel
    {
        public void OnGet() { }
    }
}
