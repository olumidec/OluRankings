using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace OluRankings.Services
{
    public sealed class SendGridEmailSender : IEmailSender
    {
        private readonly MailOptions _opts;

        public SendGridEmailSender(IOptions<MailOptions> opts)
            => _opts = opts.Value;

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrWhiteSpace(_opts.ApiKey))
                return; // safety fallback

            var client = new SendGridClient(_opts.ApiKey);

            var from = new EmailAddress(
                string.IsNullOrWhiteSpace(_opts.FromEmail) ? "no-reply@olurankings.uk" : _opts.FromEmail,
                string.IsNullOrWhiteSpace(_opts.FromName)  ? "OluRankings"             : _opts.FromName);

            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlMessage);
            var resp = await client.SendEmailAsync(msg);

            if ((int)resp.StatusCode >= 400)
            {
                var body = await resp.Body.ReadAsStringAsync();
                Console.WriteLine($"SendGrid error {(int)resp.StatusCode}: {body}");
            }
        }
    }
}
