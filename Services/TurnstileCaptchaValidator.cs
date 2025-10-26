using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace OluRankings.Services;

public sealed class TurnstileCaptchaValidator : ICaptchaValidator
{
    private readonly HttpClient _http;
    private readonly CaptchaOptions _opts;

    public TurnstileCaptchaValidator(HttpClient http, IOptions<CaptchaOptions> opts)
    {
        _http = http;
        _opts = opts.Value;
    }

    public async Task<bool> IsValidAsync(string token, string? remoteIp = null)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(_opts.SecretKey))
            return false;

        var form = new Dictionary<string, string?>
        {
            ["secret"] = _opts.SecretKey,
            ["response"] = token,
            ["remoteip"] = remoteIp
        };

        using var content = new FormUrlEncodedContent(form!);
        var resp = await _http.PostAsync("https://challenges.cloudflare.com/turnstile/v0/siteverify", content);
        if (!resp.IsSuccessStatusCode) return false;

        using var stream = await resp.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);
        return doc.RootElement.TryGetProperty("success", out var ok) && ok.GetBoolean();
    }
}
