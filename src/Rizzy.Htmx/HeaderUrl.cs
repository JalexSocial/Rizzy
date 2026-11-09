using System.Text;

namespace Rizzy.Htmx;

/// <summary>
/// Makes URLs safe to send in htmx response headers (RIZZY-016).
/// </summary>
/// <remarks>
/// HTTP header values must be ASCII; Kestrel rejects anything else. <see cref="Uri.ToString()"/>
/// returns the unescaped, human-readable form, so non-ASCII paths and queries would break the
/// response. These helpers percent-encode non-ASCII characters as UTF-8 (what browsers do) and use
/// the punycode form of internationalised host names. Existing escapes are left alone.
/// </remarks>
internal static class HeaderUrl
{
    public static string Encode(Uri url)
    {
        ArgumentNullException.ThrowIfNull(url);

        if (!url.IsAbsoluteUri)
            return Encode(url.OriginalString);

        var absolute = url.AbsoluteUri;
        if (!string.Equals(url.Host, url.IdnHost, StringComparison.Ordinal) && url.Host.Length > 0)
        {
            var hostIndex = absolute.IndexOf(url.Host, StringComparison.Ordinal);
            if (hostIndex >= 0)
                absolute = string.Concat(absolute.AsSpan(0, hostIndex), url.IdnHost, absolute.AsSpan(hostIndex + url.Host.Length));
        }

        return Encode(absolute);
    }

    public static string Encode(string url)
    {
        ArgumentNullException.ThrowIfNull(url);

        var firstNonAscii = -1;
        for (var i = 0; i < url.Length; i++)
        {
            if (url[i] > 0x7F || char.IsControl(url[i]))
            {
                firstNonAscii = i;
                break;
            }
        }

        if (firstNonAscii < 0)
            return url;

        var sb = new StringBuilder(url.Length + 16);
        sb.Append(url, 0, firstNonAscii);

        Span<byte> bytes = stackalloc byte[4];
        for (var i = firstNonAscii; i < url.Length; i++)
        {
            var ch = url[i];
            if (ch <= 0x7F && !char.IsControl(ch))
            {
                sb.Append(ch);
                continue;
            }

            int count;
            if (char.IsHighSurrogate(ch) && i + 1 < url.Length && char.IsLowSurrogate(url[i + 1]))
            {
                count = Encoding.UTF8.GetBytes(url.AsSpan(i, 2), bytes);
                i++;
            }
            else
            {
                count = Encoding.UTF8.GetBytes(url.AsSpan(i, 1), bytes);
            }

            for (var b = 0; b < count; b++)
                sb.Append('%').Append(bytes[b].ToString("X2", System.Globalization.CultureInfo.InvariantCulture));
        }

        return sb.ToString();
    }
}
