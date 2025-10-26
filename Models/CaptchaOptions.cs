namespace OluRankings.Models;
public class CaptchaOptions
{
    public string Provider { get; set; } = "Turnstile";
    public string SiteKey { get; set; } = "";
    public string SecretKey { get; set; } = "";
}
