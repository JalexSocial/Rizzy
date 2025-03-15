namespace Rizzy.Htmx.Antiforgery;

/// <summary>
/// Represents the options for Rizzy's antiforgery support.
/// </summary>
public sealed class HtmxAntiforgeryOptions
{
    /// <summary>
    /// Gets the name of the form field used for antiforgery token.
    /// </summary>
    public string FormFieldName { get; set; } = default!;

    /// <summary>
    /// Gets the name of the header used for antiforgery token.
    /// </summary>
    public string? HeaderName { get; set; }

    /// <summary>
    /// Gets the name of the cookie used for antiforgery token.
    /// </summary>
    public string CookieName { get; set; } = Constants.AntiforgeryCookieName;
}
