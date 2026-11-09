using System.Globalization;
using System.Text.RegularExpressions;

namespace Rizzy.Htmx;

/// <summary>
/// Helpers for htmx 4's per-status-code response handling (<c>hx-status:NNN</c>).
/// </summary>
/// <remarks>
/// htmx 4 swaps every response except 204 and 304, including 4xx and 5xx. Use
/// <c>hx-status:&lt;code&gt;</c> on the requesting element to change how a particular status is
/// handled, for example <c>hx-status:422="target:#errors"</c>. Patterns such as <c>"4xx"</c> and
/// <c>"40x"</c> match a range of codes. See https://four.htmx.org/reference/attributes/hx-status.
/// </remarks>
/// <example>
/// <code>
/// &lt;form hx-post="/save" hx-target="#result" @attributes="new Dictionary&lt;string, object&gt;
/// {
///     [HtmxStatus.Attribute(422)] = "target:#errors swap:innerHTML",
///     [HtmxStatus.Attribute("5xx")] = "swap:none",
/// }"&gt;
/// </code>
/// </example>
public static partial class HtmxStatus
{
    /// <summary>The status code htmx apps conventionally use for validation errors.</summary>
    public const int ValidationFailed = 422;

    /// <summary>Returns the attribute name for a specific status code, e.g. <c>hx-status:422</c>.</summary>
    /// <param name="statusCode">An HTTP status code between 100 and 599.</param>
    public static string Attribute(int statusCode)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(statusCode, 100);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(statusCode, 599);
        return "hx-status:" + statusCode.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Returns the attribute name for a status pattern: an exact code (<c>"404"</c>) or a range
    /// with trailing <c>x</c> wildcards (<c>"40x"</c>, <c>"4xx"</c>).
    /// </summary>
    /// <param name="pattern">The status pattern.</param>
    /// <exception cref="ArgumentException">The pattern is not a valid htmx status pattern.</exception>
    public static string Attribute(string pattern)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pattern);
        if (!StatusPattern().IsMatch(pattern))
            throw new ArgumentException("Expected a status code or a pattern like \"40x\" or \"4xx\".", nameof(pattern));

        return "hx-status:" + pattern.ToLowerInvariant();
    }

    [GeneratedRegex("^[1-5](?:[0-9]{2}|[0-9][xX]|[xX]{2})$")]
    private static partial Regex StatusPattern();
}
