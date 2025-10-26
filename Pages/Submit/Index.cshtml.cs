// Pages/Submit/Index.cshtml.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OluRankings.Services;
using OluRankings.Data;
using OluRankings.Models;

namespace OluRankings.Pages.Submit
{
    [Authorize] // any signed-in user
    public class IndexModel : PageModel
    {
        private readonly ICaptchaValidator _captcha;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public IndexModel(ICaptchaValidator captcha, ApplicationDbContext db, IWebHostEnvironment env)
        {
            _captcha = captcha;
            _db = db;
            _env = env;
        }

        // Form fields
        [BindProperty, Required] public string FirstName { get; set; } = string.Empty;
        [BindProperty, Required] public string LastName  { get; set; } = string.Empty;

        [BindProperty] public string School   { get; set; } = string.Empty;
        [BindProperty] public string Region   { get; set; } = string.Empty;
        [BindProperty] public string Position { get; set; } = "Guard";
        [BindProperty] public string AgeGroup { get; set; } = "U14";

        [BindProperty, Url] public string VideoUrl { get; set; } = string.Empty;

        [BindProperty] public bool Consent { get; set; }

        // **This is what the view expects**
        [BindProperty] public IFormFile? Evidence { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // CAPTCHA check
            var token = Request.Form["cf-turnstile-response"].ToString();
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            if (!await _captcha.IsValidAsync(token, remoteIp))
            {
                ModelState.AddModelError(string.Empty, "Please complete the CAPTCHA.");
                return Page();
            }

            // Optional evidence upload (JPG/PNG/PDF, <= 10MB)
            string evidencePath = "";
            string? evidenceContentType = null;
            long evidenceBytes = 0;

            if (Evidence is not null)
            {
                var allowed = new[] { "image/jpeg", "image/png", "application/pdf" };
                if (!allowed.Contains(Evidence.ContentType) || Evidence.Length > 10 * 1024 * 1024)
                {
                    ModelState.AddModelError("", "Evidence must be JPG/PNG/PDF and under 10MB.");
                    return Page();
                }

                var folder = Path.Combine(_env.WebRootPath, "evidence");
                Directory.CreateDirectory(folder);
                var fn = $"{Guid.NewGuid()}{Path.GetExtension(Evidence.FileName)}";
                var savePath = Path.Combine(folder, fn);
                using (var fs = System.IO.File.Create(savePath))
                    await Evidence.CopyToAsync(fs);

                evidencePath = $"/evidence/{fn}";
                evidenceContentType = Evidence.ContentType;
                evidenceBytes = Evidence.Length;
            }

            // Persist submission (maps to AthleteSubmission)
            _db.AthleteSubmissions.Add(new AthleteSubmission
            {
                SubmittedByUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value,
                GivenName = FirstName,
                FamilyName = LastName,
                School = string.IsNullOrWhiteSpace(School) ? null : School,
                Club = null, // map if you add a field
                ClassYear = null,
                Dob = null,
                EvidencePath = evidencePath,
                EvidenceContentType = evidenceContentType,
                EvidenceBytes = evidenceBytes
            });

            await _db.SaveChangesAsync();
            TempData["Message"] = "Profile submitted successfully!";
            return RedirectToPage("/Submit/Success");
        }
    }
}
