namespace Rizzy.Htmx;

/// <summary>
/// The htmx 4 request type sent in the <c>HX-Request-Type</c> header.
/// </summary>
public enum HtmxRequestType
{
    /// <summary>No <c>HX-Request-Type</c> header was sent (not htmx 4, or not an htmx request).</summary>
    None,

    /// <summary>The request targets part of the page and expects a fragment.</summary>
    Partial,

    /// <summary>The request targets the whole page (<c>body</c>) or uses <c>hx-select</c> and expects a full page.</summary>
    Full,

    /// <summary>The header had an unrecognised value.</summary>
    Unknown,
}
