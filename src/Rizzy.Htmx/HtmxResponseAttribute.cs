using Microsoft.AspNetCore.Mvc.Filters;

namespace Rizzy.Htmx;

/// <summary>
/// An attribute to apply HTMX response headers to an action result. This enables server-side control
/// of client-side HTMX behaviors such as redirection, refreshing, and element updates.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class HtmxResponseAttribute : Attribute, IResultFilter
{
    /// <summary>
    /// Specifies a client-side redirect to a new location without a full page reload.
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Pushes a new URL onto the browser's history stack.
    /// </summary>
    public string? PushUrl { get; set; }

    /// <summary>
    /// Prevents the browser's history from being updated.
    /// </summary>
    public bool PreventBrowserHistoryUpdate { get; set; }

    /// <summary>
    /// Prevents the browser's current URL from being updated.
    /// </summary>
    public bool PreventBrowserCurrentUrlUpdate { get; set; }

    /// <summary>
    /// Initiates a client-side redirect to a specified URL.
    /// </summary>
    public string? Redirect { get; set; }

    /// <summary>
    /// Triggers a client-side full page refresh.
    /// </summary>
    public bool Refresh { get; set; }

    /// <summary>
    /// Replaces the current URL in the browser's location bar.
    /// </summary>
    public string? ReplaceUrl { get; set; }

    /// <summary>
    /// Specifies how the response will be swapped in the client-side, using HTMX swap strategies.
    /// <see cref="SwapStyle.Default"/> (the default) means no <c>HX-Reswap</c> header is sent.
    /// </summary>
    public SwapStyle Reswap { get; set; } = SwapStyle.Default;

    /// <summary>
    /// Updates the target of the content update to a different element on the page using a CSS selector.
    /// </summary>
    public string? Retarget { get; set; }


    /// <summary>
    /// Chooses which part of the response is used for the swap, using a CSS selector.
    /// </summary>
    public string? Reselect { get; set; }

    /// <summary>
    /// Called before the action result is executed. This method applies the HTMX configurations
    /// specified by the attribute properties to the response headers.
    /// </summary>
    /// <remarks>
    /// The attribute does nothing for requests that were not made by htmx, so decorated actions
    /// still work for normal browser navigation (progressive enhancement).
    /// </remarks>
    /// <param name="context">The context in which the result is being executed.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is null.</exception>
    public void OnResultExecuting(ResultExecutingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // htmx response headers are meaningless for non-htmx requests; skip instead of failing.
        if (!context.HttpContext.Request.IsHtmx())
            return;

        var htmxResponse = new HtmxResponse(context.HttpContext);

        // Dynamically apply the HTMX response configurations based on set properties
        if (!string.IsNullOrEmpty(Location)) htmxResponse.Location(Location);
        if (!string.IsNullOrEmpty(PushUrl)) htmxResponse.PushUrl(PushUrl);
        if (PreventBrowserHistoryUpdate) htmxResponse.PreventBrowserHistoryUpdate();
        if (PreventBrowserCurrentUrlUpdate) htmxResponse.PreventBrowserCurrentUrlUpdate();
        if (!string.IsNullOrEmpty(Redirect)) htmxResponse.Redirect(Redirect);
        if (Refresh) htmxResponse.Refresh();
        if (!string.IsNullOrEmpty(ReplaceUrl)) htmxResponse.ReplaceUrl(ReplaceUrl);
        if (Reswap != SwapStyle.Default) htmxResponse.Reswap(Reswap);
        if (!string.IsNullOrEmpty(Retarget)) htmxResponse.Retarget(Retarget);
        if (!string.IsNullOrEmpty(Reselect)) htmxResponse.Reselect(Reselect);
    }

    /// <summary>
    /// Called after the action result is executed. This method is not used but is required by the interface.
    /// </summary>
    /// <param name="context">The context in which the result has been executed.</param>
    public void OnResultExecuted(ResultExecutedContext context)
    {
        // No action required
    }
}