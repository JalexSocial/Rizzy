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
    /// Gets the element that triggered the request if it exists.
    /// </summary>
    public string? Source { get; }

    /// <summary>
    /// Extracts the ID from the HTMX 4 'tag#id' format for the Target header.
    /// </summary>
    /// <remarks>
    /// Returns <c>null</c> when the target element has no id (<c>HX-Target: div</c>). htmx 4
    /// URI-encodes the id, so it is decoded here.
    /// </remarks>
    public string? TargetId => ExtractId(Target);

    /// <summary>
    /// Extracts the ID from the HTMX 4 'tag#id' format for the Source header.
    /// </summary>
    /// <remarks>
    /// Returns <c>null</c> when the source element has no id (<c>HX-Source: button</c>).
    /// </remarks>
    public string? SourceId => ExtractId(Source);

    /// <summary>
    /// Gets the htmx request type (`full` or `partial`) if available.
    /// </summary>
    public string? RequestType { get; }

    /// <summary>
    /// Gets the parsed htmx request type from the <c>HX-Request-Type</c> header.
    /// </summary>
    public HtmxRequestType Type { get; }

    /// <summary>
    /// Gets whether the response should be a fragment: an htmx request that targets part of the page
    /// (<c>HX-Request-Type: partial</c>) and is not a history restore.
    /// </summary>
    /// <remarks>
    /// htmx 4 sends <c>HX-Request-Type: full</c> when it targets <c>body</c> or uses <c>hx-select</c>,
    /// and history restores re-fetch the page with <c>HX-History-Restore-Request</c>; both need the
    /// full layout. Requests without an <c>HX-Request-Type</c> header (htmx 2 clients or hand-written
    /// fetches) that carry <c>HX-Request</c> are treated as partial, matching htmx 2 behavior.
    /// </remarks>
    public bool IsPartial => IsHtmx && !IsHistoryRestore && Type != HtmxRequestType.Full;

    /// <summary>
    /// Gets whether the response should be a full page (the inverse of <see cref="IsPartial"/>).
    /// </summary>
    public bool IsFullPage => !IsPartial;

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
        IsHistoryRestore = string.Equals(GetHxValueOrDefault(context.Request.Headers, HtmxRequestHeaderNames.HistoryRestoreRequest), "true", StringComparison.OrdinalIgnoreCase);
        CurrentURL = GetHxValueOrDefault(context.Request.Headers, HtmxRequestHeaderNames.CurrentURL, static value => Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uri) ? uri : null);
        Target = GetHxValueOrDefault(context.Request.Headers, HtmxRequestHeaderNames.Target);
        Source = GetHxValueOrDefault(context.Request.Headers, HtmxRequestHeaderNames.Source);
        RequestType = GetHxValueOrDefault(context.Request.Headers, HtmxRequestHeaderNames.RequestType);
        Type = RequestType switch
        {
            null => HtmxRequestType.None,
            _ when RequestType.Equals("partial", StringComparison.OrdinalIgnoreCase) => HtmxRequestType.Partial,
            _ when RequestType.Equals("full", StringComparison.OrdinalIgnoreCase) => HtmxRequestType.Full,
            _ => HtmxRequestType.Unknown,
        };
    }

    /// <summary>Extracts the element id from an htmx 4 <c>tag#id</c> identifier.</summary>
    private static string? ExtractId(string? identifier)
    {
        if (string.IsNullOrEmpty(identifier))
            return null;

        var hash = identifier.IndexOf('#');
        if (hash < 0 || hash == identifier.Length - 1)
            return null;

        var id = identifier[(hash + 1)..];
        try
        {
            return Uri.UnescapeDataString(id);
        }
        catch (UriFormatException)
        {
            return id;
        }
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
