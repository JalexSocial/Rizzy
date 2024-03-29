﻿using Microsoft.AspNetCore.Http;

namespace Rizzy.Http;

/// <summary>
/// Htmx request headers
/// </summary>
/// <param name="context"></param>
public class HtmxRequest(HttpContext context)
{
    /// <summary>
    /// Gets whether or not the current request is an Htmx triggered request.
    /// </summary>
    public bool IsHtmx => context.Request.Headers.ContainsKey(HtmxRequestHeaderNames.HtmxRequest);

    /// <summary>
    /// Gets whether or not the current request is an request initiated via an element using hx-boost.
    /// </summary>
    public bool IsBoosted => IsHtmx && context.Request.Headers.ContainsKey(HtmxRequestHeaderNames.Boosted);

    /// <summary>
    /// Gets whether or not the current request is an Htmx history restore request.
    /// </summary>
    public bool IsHistoryRestore => IsHtmx && context.Request.Headers.ContainsKey(HtmxRequestHeaderNames.HistoryRestoreRequest);

    /// <summary>
    /// Gets the current URL of the browser.
    /// </summary>
    public Uri? CurrentURL
        => IsHtmx
        && context.Request.Headers.TryGetValue(HtmxRequestHeaderNames.CurrentURL, out var values)
        && values.Count > 0
        && Uri.TryCreate(values[0], UriKind.RelativeOrAbsolute, out var uri)
        ? uri
        : null;

    /// <summary>
    /// Gets the `id` of the target element if it exists.
    /// </summary>
    public string? Target
        => IsHtmx
        && context.Request.Headers.TryGetValue(HtmxRequestHeaderNames.Target, out var values)
        && values.Count > 0
        ? values[0]
        : null;

    /// <summary>
    /// Gets the `name` of the triggered element if it exists.
    /// </summary>
    public string? TriggerName
        => IsHtmx
        && context.Request.Headers.TryGetValue(HtmxRequestHeaderNames.TriggerName, out var values)
        && values.Count > 0
        ? values[0]
        : null;

    /// <summary>
    /// Gets the `id` of the triggered element if it exists.
    /// </summary>
    public string? Trigger
        => IsHtmx
        && context.Request.Headers.TryGetValue(HtmxRequestHeaderNames.Trigger, out var values)
        && values.Count > 0
        ? values[0]
        : null;

    /// <summary>
    /// Gets the user response to an hx-prompt, if any.
    /// </summary>
    public string? Prompt => IsHtmx
        && context.Request.Headers.TryGetValue(HtmxRequestHeaderNames.Prompt, out var values)
        && values.Count > 0
        ? values[0]
        : null;
}
