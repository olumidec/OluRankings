using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace OluRankings.Services
{
    public class NoOpEmailSender : IEmailSender
    {
        private readonly ILogger<NoOpEmailSender> _logger;
        public NoOpEmailSender(ILogger<NoOpEmailSender> logger) => _logger = logger;

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation("NoOpEmailSender: would send to {Email} :: {Subject}", email, subject);
            return Task.CompletedTask;
        }
    }
}
