using Microsoft.AspNetCore.Http;

namespace Rizzy.Htmx;

/// <summary>
/// Htmx request headers
/// </summary>
public class HtmxRequest
{
    /// <summary>
    /// Gets the HTTP method of the current request.
    /// </summary>
    public string Method { get; }

    /// <summary>
    /// Gets the HTTP method of the current request.
    /// </summary>
    public PathString Path { get; }

    /// <summary>
    /// Gets whether or not the current request is an Htmx triggered request.
    /// </summary>
    public bool IsHtmx { get; }

    /// <summary>
    /// Gets whether or not the current request is an request initiated via an element using hx-boost.
    /// </summary>
    public bool IsBoosted { get; }

    /// <summary>
    /// Gets whether or not the current request is an Htmx history restore request.
    /// </summary>
    public bool IsHistoryRestore { get; }

    /// <summary>
    /// Gets the current URL of the browser.
    /// </summary>
    public Uri? CurrentURL { get; }

    /// <summary>
    /// Gets the `id` of the target element if it exists.
    /// </summary>
    public string? Target { get; }

    /// <summary>
    /// Gets the `name` of the triggered element if it exists.
    /// </summary>
    public string? TriggerName { get; }

    /// <summary>
    /// Gets the `id` of the triggered element if it exists.
    /// </summary>
    public string? Trigger { get; }

    /// <summary>
    /// Gets the user response to an hx-prompt, if any.
    /// </summary>
    public string? Prompt { get; }

    /// <summary>
    /// Creates a new instance of <see cref="HtmxRequest"/>.
    /// </summary>
    public HtmxRequest(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        Method = context.Request.Method;
        Path = context.Request.Path;
        var isHtmx = IsHtmx = context.Request.Headers.ContainsKey(HtmxRequestHeaderNames.HtmxRequest);

        IsBoosted = context.Request.Headers.ContainsKey(HtmxRequestHeaderNames.Boosted);
        IsHistoryRestore = context.Request.Headers.ContainsKey(HtmxRequestHeaderNames.HistoryRestoreRequest);
        CurrentURL = GetHxValueOrDefault(context.Request.Headers, HtmxRequestHeaderNames.CurrentURL, static value => Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uri) ? uri : null);
        Target = GetHxValueOrDefault(context.Request.Headers, HtmxRequestHeaderNames.Target);
        TriggerName = GetHxValueOrDefault(context.Request.Headers, HtmxRequestHeaderNames.TriggerName);
        Trigger = GetHxValueOrDefault(context.Request.Headers, HtmxRequestHeaderNames.Trigger);
        Prompt = GetHxValueOrDefault(context.Request.Headers, HtmxRequestHeaderNames.Prompt);
    }

    private static string? GetHxValueOrDefault(IHeaderDictionary headers, string key)
        => headers.TryGetValue(key, out var values)
        && values.Count > 0
        && values[0] is var value
        && !string.IsNullOrWhiteSpace(value)
        ? value.Trim()
        : null;

    private static T? GetHxValueOrDefault<T>(IHeaderDictionary headers, string key, Func<string, T?> factory)
        => GetHxValueOrDefault(headers, key) is string value
        ? factory.Invoke(value)
        : default(T);
}
