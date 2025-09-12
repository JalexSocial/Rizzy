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

    /// <summary>
    /// Serializes .NET objects for use with Alpine.js x-data attribute
    /// </summary>
    /// <param name="value"></param>
    /// <param name="ignoreNullValues"></param>
    /// <code>
    /// <Widget x-data="@myDataObject.SerializeAsAlpineData()"></Widget>
    /// </code>
    /// <returns></returns>
    public static string SerializeAsAlpineData(this object? value, bool ignoreNullValues = false)
    {
        if (value is null)
            return string.Empty;

        var options = new JsonSerializerOptions
        {
            Converters = { new SingleQuoteStringConverter() },
            DefaultIgnoreCondition = ignoreNullValues ? JsonIgnoreCondition.WhenWritingNull : JsonIgnoreCondition.Never 
        };

        return JsonSerializer.Serialize(value, options);
    }
}
