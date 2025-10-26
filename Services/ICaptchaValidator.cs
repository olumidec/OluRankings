using System.Threading.Tasks;

namespace OluRankings.Services;

public interface ICaptchaValidator
{
    Task<bool> IsValidAsync(string token, string? remoteIp = null);
}
