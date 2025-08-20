using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[Route("lang")]
public class LangController : Controller
{
    private static readonly HashSet<string> Supported = new(StringComparer.OrdinalIgnoreCase)
    { "en-US", "km-KH" };

    [HttpPost("set")]
    // Add [ValidateAntiForgeryToken] if your form includes an antiforgery token
    public IActionResult Set(string culture, string? returnUrl = "/")
    {
        culture = Normalize(culture);
        if (!Supported.Contains(culture)) culture = "en-US";

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true, SameSite = SameSiteMode.Lax }
        );

        return LocalRedirect(IsLocal(returnUrl) ? returnUrl! : "/");
    }

    // GET fallback so links can switch: /lang/set?culture=en-US
    [HttpGet("set")]
    public IActionResult SetGet(string culture, string? returnUrl = "/") => Set(culture, returnUrl);

    private static string Normalize(string? c) => (c ?? "").ToLowerInvariant() switch
    {
        "en" or "en-us" => "en-US",
        "km" or "km-kh" => "km-KH",
        _ => "en-US"
    };

    private static bool IsLocal(string? url) =>
        !string.IsNullOrWhiteSpace(url) && Uri.IsWellFormedUriString(url, UriKind.Relative);
}
