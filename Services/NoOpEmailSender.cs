using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace OluRankings.Services;

public sealed class NoOpEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
        => Task.CompletedTask;
}
