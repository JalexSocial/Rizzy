using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Rizzy.Htmx;

/// <summary>
/// Options for <see cref="HtmxErrorHandlingExtensions.UseHtmxErrorHandling(IApplicationBuilder, Action{HtmxErrorHandlingOptions}?)"/>.
/// </summary>
public sealed class HtmxErrorHandlingOptions
{
    /// <summary>
    /// A CSS selector for an error region. When set, server errors on partial htmx requests are
    /// retargeted into it (<c>HX-Retarget</c> + <c>HX-Reswap: innerHTML</c>). When null (the
    /// default) the error response is not swapped (<c>HX-Reswap: none</c>).
    /// </summary>
    public string? ErrorTarget { get; set; }

    /// <summary>
    /// The client event triggered for server errors on partial htmx requests (detail:
    /// <c>{ "status": 500 }</c>). Set to null to trigger nothing. Defaults to <c>rz:server-error</c>.
    /// </summary>
    public string? ErrorEvent { get; set; } = "rz:server-error";
}

/// <summary>
/// htmx-aware handling of error and authentication responses (RIZZY-019).
/// </summary>
/// <remarks>
/// htmx 4 swaps 4xx and 5xx responses like any other. That is right for validation errors (return
/// 422 with the form, see <see cref="HtmxResponse.ValidationFailed"/> and <see cref="HtmxStatus"/>),
/// but an exception page or a login page should not be swapped into a fragment target.
/// </remarks>
public static class HtmxErrorHandlingExtensions
{
    /// <summary>
    /// Keeps server errors (5xx) on partial htmx requests from being swapped into the request's
    /// target. Responses that already set <c>HX-Retarget</c>, <c>HX-Reswap</c>, <c>HX-Redirect</c>,
    /// <c>HX-Refresh</c> or <c>HX-Location</c> are left alone.
    /// </summary>
    /// <remarks>
    /// Register it before <c>UseExceptionHandler</c>/<c>UseStatusCodePages</c> so it also covers
    /// the responses they produce. Full-page htmx requests (boosted navigation, history restore)
    /// are not changed.
    /// </remarks>
    public static IApplicationBuilder UseHtmxErrorHandling(this IApplicationBuilder app, Action<HtmxErrorHandlingOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(app);

        var options = new HtmxErrorHandlingOptions();
        configure?.Invoke(options);

        return app.Use(next => context =>
        {
            if (context.Request.Htmx().IsPartial)
            {
                context.Response.OnStarting(static state =>
                {
                    var (ctx, opts) = ((HttpContext, HtmxErrorHandlingOptions))state;
                    ApplyServerErrorHeaders(ctx, opts);
                    return Task.CompletedTask;
                }, (context, options));
            }

            return next(context);
        });
    }

    private static void ApplyServerErrorHeaders(HttpContext context, HtmxErrorHandlingOptions options)
    {
        if (context.Response.StatusCode < 500)
            return;

        var headers = context.Response.Headers;
        if (headers.ContainsKey(HtmxResponseHeaderNames.Retarget) ||
            headers.ContainsKey(HtmxResponseHeaderNames.Reswap) ||
            headers.ContainsKey(HtmxResponseHeaderNames.Redirect) ||
            headers.ContainsKey(HtmxResponseHeaderNames.Refresh) ||
            headers.ContainsKey(HtmxResponseHeaderNames.Location))
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(options.ErrorTarget))
        {
            headers[HtmxResponseHeaderNames.Retarget] = options.ErrorTarget;
            headers[HtmxResponseHeaderNames.Reswap] = Constants.SwapStyles.InnerHTML;
        }
        else
        {
            headers[HtmxResponseHeaderNames.Reswap] = Constants.SwapStyles.None;
        }

        if (!string.IsNullOrWhiteSpace(options.ErrorEvent))
            context.Response.Htmx().Trigger(options.ErrorEvent, new { status = context.Response.StatusCode });
    }

    /// <summary>
    /// Makes cookie authentication answer htmx requests with <c>HX-Redirect</c> instead of a 302 to
    /// the login or access-denied page. A 302 would be followed by <c>fetch</c> and the login page
    /// swapped into the target; <c>HX-Redirect</c> navigates the whole browser window instead.
    /// </summary>
    /// <remarks>
    /// Responds with 401 (login) or 403 (access denied) and an empty body. Non-htmx requests keep
    /// the configured behavior. Existing <c>OnRedirectToLogin</c>/<c>OnRedirectToAccessDenied</c>
    /// handlers still run for non-htmx requests.
    /// </remarks>
    /// <example>
    /// <code>
    /// builder.Services.AddAuthentication().AddCookie(o => o.Events.UseHtmxRedirects());
    /// </code>
    /// </example>
    public static CookieAuthenticationEvents UseHtmxRedirects(this CookieAuthenticationEvents events)
    {
        ArgumentNullException.ThrowIfNull(events);

        var onLogin = events.OnRedirectToLogin;
        var onAccessDenied = events.OnRedirectToAccessDenied;

        events.OnRedirectToLogin = context => RedirectHtmx(context, StatusCodes.Status401Unauthorized) ?? onLogin(context);
        events.OnRedirectToAccessDenied = context => RedirectHtmx(context, StatusCodes.Status403Forbidden) ?? onAccessDenied(context);

        return events;
    }

    private static Task? RedirectHtmx(RedirectContext<CookieAuthenticationOptions> context, int statusCode)
    {
        if (!context.Request.IsHtmx())
            return null;

        context.Response.StatusCode = statusCode;
        context.Response.Headers[HtmxResponseHeaderNames.Redirect] = HeaderUrl.Encode(context.RedirectUri);
        return Task.CompletedTask;
    }
}
