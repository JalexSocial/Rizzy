using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
#if NET11_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
#endif

namespace Rizzy;

/// <summary>
/// Decides whether a posted form may be supplied to Blazor form mapping
/// (<c>[SupplyParameterFromForm]</c>) for a Rizzy view.
/// </summary>
/// <remarks>
/// <para>
/// ASP.NET Core enforces antiforgery when an endpoint binds a form itself (MVC antiforgery filters,
/// Minimal API <c>[FromForm]</c> parameters, Razor component endpoints). Rizzy reads the form for
/// its components on the endpoint's behalf, so it applies the same rule:
/// </para>
/// <list type="number">
/// <item>Endpoints that opt out (<c>.DisableAntiforgery()</c>, <c>[IgnoreAntiforgeryToken]</c>) are
/// allowed.</item>
/// <item>If antiforgery already judged the request (<c>app.UseAntiforgery()</c>, or on .NET 11 the
/// automatic CSRF middleware, for endpoints that require antiforgery), that verdict is used.</item>
/// <item>On .NET 11, other requests are checked by the framework's automatic CSRF protection
/// (<c>ICsrfProtection</c>, based on the browser's <c>Sec-Fetch-Site</c> and <c>Origin</c>
/// headers): same-origin posts, which is every htmx request, are allowed without a token, and
/// cross-site posts are rejected. On .NET 10 there is no such check and the request is allowed, as
/// before.</item>
/// </list>
/// </remarks>
internal static class FormPostProtection
{
    public static async ValueTask<bool> IsAllowedAsync(HttpContext context)
    {
        if (HttpMethods.IsGet(context.Request.Method) || HttpMethods.IsHead(context.Request.Method))
            return true;

        if (context.GetEndpoint()?.Metadata.GetMetadata<IAntiforgeryMetadata>() is { RequiresValidation: false })
            return true;

        if (context.Features.Get<IAntiforgeryValidationFeature>() is { } verdict)
            return verdict.IsValid;

#if NET11_0_OR_GREATER
        var csrf = context.RequestServices.GetService<ICsrfProtection>();
        return csrf is null || (await csrf.ValidateAsync(context).ConfigureAwait(false)).IsAllowed;
#else
        await Task.CompletedTask.ConfigureAwait(false);
        return true;
#endif
    }
}
