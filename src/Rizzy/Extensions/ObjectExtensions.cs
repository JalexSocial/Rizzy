using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.ComponentModel;

namespace Rizzy.Extensions;

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
    /// Credit to Alexander Zeitler for developing this approach. It requires Newtonsoft because System.Text.Json doesn't allow non-standard quote characters.
    /// </summary>
    /// <seealso>See Serializing .NET objects for use with Alpine.js x-data attribute
    ///     <cref>https://alexanderzeitler.com/articles/serializing-dotnet-csharp-object-for-use-with-alpine-x-data-attribute/</cref>
    /// </seealso>
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

        using StringWriter stringWriter = new StringWriter();
        using JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter)stringWriter)
        {
            QuoteName = false,
            QuoteChar = '\''
        };

        Newtonsoft.Json.JsonSerializer jsonSerializer = new()
        {
            ContractResolver = (IContractResolver)new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = ignoreNullValues ? NullValueHandling.Ignore : NullValueHandling.Include
        };

        jsonSerializer.Serialize((JsonWriter)jsonTextWriter, value);

        return stringWriter.ToString();
    }
}
