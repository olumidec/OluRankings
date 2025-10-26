using Microsoft.AspNetCore.Identity;

namespace OluRankings.Models;

public class ApplicationUser : IdentityUser
{
    // Optional: add profile fields you’ll need later
    public string? DisplayName { get; set; }
    // For coaches: which team they manage (Authorization policy uses this)
    public string? TeamId { get; set; }

    public ApplicationUser? UserEntity { get; set; }

}
