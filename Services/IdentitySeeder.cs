using Microsoft.AspNetCore.Identity;
using OluRankings.Models;

namespace OluRankings.Services;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = ["Admin","Coach","Ranker","User"];
        foreach (var r in roles)
            if (!await roleMgr.RoleExistsAsync(r))
                await roleMgr.CreateAsync(new IdentityRole(r));

        // TODO: put your real admin email/password here or use secrets/env
        const string adminEmail = "admin@olurankings.uk";
        const string adminPass  = "ChangeMe!123";

        var admin = await userMgr.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                DisplayName = "Site Admin"
            };
            var res = await userMgr.CreateAsync(admin, adminPass);
            if (res.Succeeded)
                await userMgr.AddToRoleAsync(admin, "Admin");
        }

        // Example coach with a TeamId claim/property
        const string coachEmail = "coach@example.com";
        const string coachPass  = "ChangeMe!123";
        var coach = await userMgr.FindByEmailAsync(coachEmail);
        if (coach is null)
        {
            coach = new ApplicationUser
            {
                UserName = coachEmail,
                Email = coachEmail,
                EmailConfirmed = true,
                DisplayName = "Demo Coach",
                TeamId = "TEAM-LDN-U16"
            };
            var res = await userMgr.CreateAsync(coach, coachPass);
            if (res.Succeeded)
                await userMgr.AddToRoleAsync(coach, "Coach");
        }

        // Example ranker
        const string rankerEmail = "ranker@example.com";
        const string rankerPass  = "ChangeMe!123";
        var ranker = await userMgr.FindByEmailAsync(rankerEmail);
        if (ranker is null)
        {
            ranker = new ApplicationUser
            {
                UserName = rankerEmail,
                Email = rankerEmail,
                EmailConfirmed = true,
                DisplayName = "Demo Ranker"
            };
            var res = await userMgr.CreateAsync(ranker, rankerPass);
            if (res.Succeeded)
                await userMgr.AddToRoleAsync(ranker, "Ranker");
        }
    }
}
