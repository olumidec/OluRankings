// Program.cs
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

using OluRankings.Identity;
using OluRankings.Models;
using OluRankings.Services;
using OluRankings.Data; // ApplicationDbContext

// Type aliases
using CaptchaOptions = OluRankings.Services.CaptchaOptions;
using NoOpEmailSender = OluRankings.Services.NoOpEmailSender;

var builder = WebApplication.CreateBuilder(args);
var skipDb = builder.Configuration.GetValue<bool>("SkipDb");

// 1) Razor Pages
builder.Services.AddRazorPages();

// 2) Cloudflare Turnstile
builder.Services.AddHttpClient();
builder.Services.Configure<CaptchaOptions>(builder.Configuration.GetSection("Captcha"));
builder.Services.AddSingleton<ICaptchaValidator, TurnstileCaptchaValidator>();

// 3) Databases (IdentityDb + AppDb)
builder.Services.AddDbContext<IdentityDb>(opts =>
{
    var cs = builder.Configuration.GetConnectionString("IdentityDb");
    if (builder.Environment.IsDevelopment())
        opts.UseSqlite(cs);
    else
        opts.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")!);
});

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
{
    if (builder.Environment.IsDevelopment())
        opts.UseSqlite(builder.Configuration.GetConnectionString("AppDb")!);
    else
        opts.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")!);
});

// 4) Identity (+ Roles)
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = !builder.Environment.IsDevelopment();
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddEntityFrameworkStores<IdentityDb>()
    .AddDefaultTokenProviders();

// Claims factory (TeamId → claim)
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, TeamIdClaimsPrincipalFactory>();

// Email sender (stub for now)
builder.Services.AddSingleton<IEmailSender, NoOpEmailSender>();

// 6) Cookies
builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/Identity/Account/Login";
    o.AccessDeniedPath = "/AccessDenied";
    o.SlidingExpiration = true;
});

// 7) Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin",  p => p.RequireRole("Admin"));
    options.AddPolicy("RequireRanker", p => p.RequireRole("Ranker"));
    options.AddPolicy("RequireCoach",  p => p.RequireRole("Coach"));

    options.AddPolicy("CanManageOwnTeam", p =>
        p.RequireAssertion(ctx =>
        {
            var user = ctx.User;
            if (!user.IsInRole("Coach")) return false;

            var http = ctx.Resource as HttpContext;
            var routeTeamId = http?.Request.RouteValues.TryGetValue("teamId", out var v) == true
                ? v?.ToString()
                : null;

            var claimTeamId = user.Claims.FirstOrDefault(c => c.Type == "TeamId")?.Value;

            return !string.IsNullOrEmpty(routeTeamId)
                   && !string.IsNullOrEmpty(claimTeamId)
                   && routeTeamId == claimTeamId;
        })
    );
});

// 8) (Optional) App Insights
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// 9) Migrate & seed (both DBs) — OPTIONAL
if (!skipDb)
{
    using var scope = app.Services.CreateScope();
    try
    {
        var idDb  = scope.ServiceProvider.GetRequiredService<IdentityDb>();
        await idDb.Database.MigrateAsync();

        var appDb = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await appDb.Database.MigrateAsync();

        await IdentitySeeder.SeedAsync(scope.ServiceProvider);
        Console.WriteLine("✅ DB migrations & seed completed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ DB startup/migration failed:");
        Console.WriteLine(ex.ToString());
        throw;
    }
}
else
{
    Console.WriteLine("⚠️ Skipping database initialization (SkipDb=true).");
}

// 10) Prod middlewares
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();

    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.Run();
