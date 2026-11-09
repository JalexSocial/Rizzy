using System.Globalization;

namespace Rizzy.Htmx;

/// <summary>
/// Formatting helpers for htmx 4 modifier values (HCON, htmx's mini config language used by
/// <c>hx-trigger</c>, <c>hx-swap</c> and friends).
/// </summary>
/// <remarks>
/// htmx 4 reads a bare modifier value up to the next whitespace or comma, so any value that
/// contains either must be quoted (<c>from:'closest form'</c>). The htmx 2 brace syntax
/// (<c>from:{closest form}</c>) is no longer recognised. All numbers are formatted with the
/// invariant culture.
/// </remarks>
internal static class HconFormat
{
    /// <summary>
    /// Formats a duration as an htmx interval: whole seconds as <c>Ns</c>, anything else as whole
    /// milliseconds (<c>1500ms</c>), independent of the current culture.
    /// </summary>
    public static string Interval(TimeSpan time)
    {
        if (time < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(time), time, "htmx intervals cannot be negative.");

        var milliseconds = (long)Math.Round(time.TotalMilliseconds, MidpointRounding.AwayFromZero);

        return milliseconds >= 1000 && milliseconds % 1000 == 0
            ? (milliseconds / 1000).ToString(CultureInfo.InvariantCulture) + "s"
            : milliseconds.ToString(CultureInfo.InvariantCulture) + "ms";
    }

    /// <summary>Formats a millisecond count as an htmx interval.</summary>
    public static string Interval(int milliseconds) => Interval(TimeSpan.FromMilliseconds(milliseconds));

    /// <summary>Formats a number with the invariant culture.</summary>
    public static string Number(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);

    /// <summary>
    /// Formats a modifier value (usually a CSS or extended selector), quoting it when it contains
    /// whitespace or commas. Values already wrapped in quotes are kept; the legacy htmx 2
    /// <c>{...}</c> wrapper is converted to quotes.
    /// </summary>
    /// <exception cref="ArgumentException">The value needs quoting but contains both quote characters.</exception>
    public static string Value(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        value = value.Trim();

        if (value.Length >= 2)
        {
            var first = value[0];
            var last = value[^1];

            if ((first == '\'' && last == '\'') || (first == '"' && last == '"'))
                return value;

            if (first == '{' && last == '}')
                value = value[1..^1].Trim();
        }

        if (!NeedsQuoting(value))
            return value;

        if (!value.Contains('\''))
            return "'" + value + "'";

        if (!value.Contains('"'))
            return "\"" + value + "\"";

        throw new ArgumentException(
            "An htmx modifier value containing whitespace or commas cannot contain both single and double quotes.",
            nameof(value));
    }

    private static bool NeedsQuoting(string value)
    {
        foreach (var ch in value)
        {
            if (char.IsWhiteSpace(ch) || ch == ',')
                return true;
        }

        return value.Length > 0 && (value[0] == '\'' || value[0] == '"');
    }
}
