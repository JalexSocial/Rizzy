using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Rizzy.Configuration;

namespace Rizzy.Antiforgery;

/// <summary>
/// This will add a HX-XSRF-TOKEN to each response, no matter if it was initiated by HTMX or not.
/// </summary>
internal sealed class HtmxAntiforgeryMiddleware(IAntiforgery antiforgery, 
	IOptions<HtmxAntiforgeryOptions> antiforgeryConfig, 
	IOptions<RizzyConfig> rizzyConfig,
	RequestDelegate next)
{
    private static readonly CookieOptions cookieOptions = new CookieOptions
    {
        HttpOnly = false,
        SameSite = SameSiteMode.Strict,
        IsEssential = true
    };

    public async Task Invoke(HttpContext context)
    {
        if (rizzyConfig.Value.AntiforgeryStrategy == AntiforgeryStrategy.GenerateTokensPerRequest)
        {
            var tokens = antiforgery.GetAndStoreTokens(context);
            context.Response.Cookies.Append(antiforgeryConfig.Value.CookieName, tokens.RequestToken!, cookieOptions);
        }

        await next.Invoke(context);
	}
}