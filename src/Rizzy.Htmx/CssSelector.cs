using System.Globalization;
using System.Text;

namespace Rizzy.Htmx;

/// <summary>
/// CSS selector helpers (RIZZY-031).
/// </summary>
public static class CssSelector
{
    /// <summary>
    /// Escapes a value for use as a CSS identifier, following the CSSOM <c>CSS.escape()</c> algorithm.
    /// </summary>
    /// <example><c>CssSelector.Escape("item:1")</c> returns <c>item\:1</c>; <c>"1st"</c> returns <c>\31 st</c>.</example>
    public static string Escape(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var sb = new StringBuilder(value.Length + 8);
        for (var i = 0; i < value.Length; i++)
        {
            var ch = value[i];

            if (ch == '\0')
            {
                sb.Append('�');
            }
            else if ((ch >= '\u0001' && ch <= '\u001F') || ch == '\u007F' ||
                     (i == 0 && ch >= '0' && ch <= '9') ||
                     (i == 1 && ch >= '0' && ch <= '9' && value[0] == '-'))
            {
                sb.Append('\\').Append(((int)ch).ToString("x", CultureInfo.InvariantCulture)).Append(' ');
            }
            else if (i == 0 && ch == '-' && value.Length == 1)
            {
                sb.Append('\\').Append(ch);
            }
            else if (ch >= 0x80 || ch == '-' || ch == '_' ||
                     (ch >= '0' && ch <= '9') || (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z'))
            {
                sb.Append(ch);
            }
            else
            {
                sb.Append('\\').Append(ch);
            }
        }

        return sb.ToString();
    }

    /// <summary>Returns an id selector (<c>#id</c>) with the id escaped.</summary>
    public static string Id(string id)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        return "#" + Escape(id);
    }
}
