using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace OluRankings.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly MailOptions _opts;
        private readonly ILogger<SendGridEmailSender> _logger;

        public SendGridEmailSender(IOptions<MailOptions> opts, ILogger<SendGridEmailSender> logger)
        {
            _opts = opts.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrWhiteSpace(_opts.ApiKey))
            {
                _logger.LogWarning("SENDGRID_API_KEY missing; skipping email to {Email}", email);
                return;
            }

            var client = new SendGridClient(_opts.ApiKey);
            var from   = new EmailAddress(_opts.FromEmail ?? "no-reply@olurankings.uk",
                                          _opts.FromName  ?? "OluRankings");
            var to     = new EmailAddress(email);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent: htmlMessage);
            var resp = await client.SendEmailAsync(msg);

            if ((int)resp.StatusCode >= 400)
            {
                var body = await resp.Body.ReadAsStringAsync();
                _logger.LogError("SendGrid send failed ({Status}): {Body}", resp.StatusCode, body);
            }
        }
    }
}
