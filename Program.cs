// Program.cs
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

using OluRankings.Identity;
using OluRankings.Models;
using OluRankings.Services;
using OluRankings.Data; // ApplicationDbContext

// Aliases
using CaptchaOptions = OluRankings.Services.CaptchaOptions;
using NoOpEmailSender = OluRankings.Services.NoOpEmailSender;

var builder = WebApplication.CreateBuilder(args);

// Toggle DB init/migrate with env var SkipDb=true
var skipDb = builder.Configuration.GetValue<bool>("SkipDb");

// --------------------------- Services ---------------------------------

// Razor Pages
builder.Services.AddRazorPages();

// HTTP client (Turnstile + SendGrid client wrappers)
builder.Services.AddHttpClient();

// Cloudflare Turnstile
builder.Services.Configure<CaptchaOptions>(builder.Configuration.GetSection("Captcha"));
builder.Services.AddSingleton<ICaptchaValidator, TurnstileCaptchaValidator>();

// Databases (Identity + App)
builder.Services.AddDbContext<IdentityDb>(opts =>
{
    var cs = builder.Configuration.GetConnectionString("IdentityDb");
    if (builder.Environment.IsDevelopment()) opts.UseSqlite(cs);
    else opts.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")!);
});

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
{
    if (builder.Environment.IsDevelopment())
        opts.UseSqlite(builder.Configuration.GetConnectionString("AppDb")!);
    else
        opts.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")!);
});

// Identity (+Roles)
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

// --------------------------- Email (SendGrid / No-Op) -----------------

// Bind mail options from env (Render UI):
// SENDGRID_API_KEY, MAIL__FROM_EMAIL, MAIL__FROM_NAME
builder.Services.Configure<MailOptions>(o =>
{
    o.ApiKey    = builder.Configuration["SENDGRID_API_KEY"];
    o.FromEmail = builder.Configuration["MAIL:FROM_EMAIL"];
    o.FromName  = builder.Configuration["MAIL:FROM_NAME"];
});

// Choose the implementation automatically
if (!string.IsNullOrWhiteSpace(builder.Configuration["SENDGRID_API_KEY"]))
{
    builder.Services.AddSingleton<IEmailSender, SendGridEmailSender>();
}
else
{
    builder.Services.AddSingleton<IEmailSender, NoOpEmailSender>();
}

// Cookies
builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/Identity/Account/Login";
    o.AccessDeniedPath = "/AccessDenied";
    o.SlidingExpiration = true;
    o.Cookie.SameSite = SameSiteMode.Lax;
});

// Authorization policies
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
            var ok = http?.Request.RouteValues.TryGetValue("teamId", out var v) ?? false;
            var routeTeamId = ok ? v?.ToString() : null;
            var claimTeamId = user.Claims.FirstOrDefault(c => c.Type == "TeamId")?.Value;

            return !string.IsNullOrEmpty(routeTeamId)
                   && !string.IsNullOrEmpty(claimTeamId)
                   && routeTeamId == claimTeamId;
        })
    );
});

// (Optional) App Insights
builder.Services.AddApplicationInsightsTelemetry();

// --------------------------- Pipeline ---------------------------------

var app = builder.Build();

// DB migrate/seed (disable with SkipDb=true)
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
        Console.WriteLine(ex);
        throw;
    }
}
else
{
    Console.WriteLine("⚠️ Skipping database initialization (SkipDb=true).");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();

    // Trust Cloudflare/Render proxy headers
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
        RequireHeaderSymmetry = false,
        ForwardLimit = null
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.Run();
