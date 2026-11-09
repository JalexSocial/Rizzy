using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Rizzy.Htmx;

namespace Rizzy;

/// <summary>
/// Adds the htmx request headers to the response <c>Vary</c> header (RIZZY-018).
/// </summary>
internal static class HtmxVaryExtensions
{
    private static readonly string[] HtmxVaryHeaders =
    [
        HtmxRequestHeaderNames.HtmxRequest,
        HtmxRequestHeaderNames.RequestType,
        HtmxRequestHeaderNames.HistoryRestoreRequest,
    ];

    /// <summary>
    /// Ensures <c>Vary</c> lists <c>HX-Request</c>, <c>HX-Request-Type</c> and
    /// <c>HX-History-Restore-Request</c>, keeping any values already present.
    /// </summary>
    /// <remarks>
    /// The same URL returns a full page or a fragment depending on these headers, so both shapes
    /// must vary on them. Otherwise a browser or proxy cache can serve a fragment for a normal
    /// navigation (or the reverse).
    /// </remarks>
    public static void VaryByHtmxHeaders(this HttpResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (response.HasStarted)
            return;

        var existing = response.Headers.Vary;
        var values = new List<string>();
        foreach (var value in existing)
        {
            if (string.IsNullOrEmpty(value))
                continue;

            foreach (var token in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                values.Add(token);
        }

        if (values.Contains("*"))
            return;

        var changed = false;
        foreach (var header in HtmxVaryHeaders)
        {
            if (!values.Exists(v => string.Equals(v, header, StringComparison.OrdinalIgnoreCase)))
            {
                values.Add(header);
                changed = true;
            }
        }

        if (changed)
            response.Headers.Vary = new StringValues(string.Join(", ", values));
    }
}
