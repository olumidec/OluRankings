using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OluRankings.Pages.Teams;

[Authorize(Policy = "CanManageOwnTeam")]
public class ManageModel : PageModel
{
    public string TeamId { get; set; } = string.Empty;

    public void OnGet(string teamId)
    {
        TeamId = teamId;
        // Load team data later (DB) — for now, render read-only UI
    }
}
