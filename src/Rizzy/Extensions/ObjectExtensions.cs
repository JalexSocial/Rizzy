using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rizzy;

public static class ObjectExtensions
{
    /// <summary>
    /// Converts a POCO object to a dictionary where each key/value is a property from the original POCO object
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static Dictionary<string, object?> ToDictionary(this object? values)
    {
        var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        if (values != null)
        {
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(values))
            {
                if (propertyDescriptor is null) continue;

                object? obj = propertyDescriptor.GetValue(values);
                dict.Add(propertyDescriptor.Name, obj);
            }
        }

        return dict;
    }

    // The default System.Text.Json encoder escapes HTML-sensitive characters (<, >, &, ', +, `),
    // backslashes, control characters and line/paragraph separators, so the output is always a
    // single valid JavaScript expression that cannot break out of a string literal.
    private static readonly JsonSerializerOptions AlpineDataOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never
    };

    private static readonly JsonSerializerOptions AlpineDataIgnoreNullOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Serializes .NET objects for use with an Alpine.js <c>x-data</c> attribute.
    /// </summary>
    /// <remarks>
    /// The result is standard, HTML-safe JSON (a valid JavaScript object literal). Razor
    /// attribute-encodes the double quotes, and the browser decodes them before Alpine
    /// evaluates the expression, so untrusted string values cannot inject script.
    /// </remarks>
    /// <param name="value">The object to serialize.</param>
    /// <param name="ignoreNullValues">When <see langword="true"/>, properties with null values are omitted.</param>
    /// <code>
    /// <Widget x-data="@myDataObject.SerializeAsAlpineData()"></Widget>
    /// </code>
    /// <returns>A JSON string, or an empty string when <paramref name="value"/> is null.</returns>
    public static string SerializeAsAlpineData(this object? value, bool ignoreNullValues = false)
    {
        if (value is null)
            return string.Empty;

        return JsonSerializer.Serialize(value, ignoreNullValues ? AlpineDataIgnoreNullOptions : AlpineDataOptions);
    }
}
