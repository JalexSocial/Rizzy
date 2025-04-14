using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rizzy;

internal class SingleQuoteStringConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Reading logic (not relevant for serialization)
        return reader.GetString() ?? string.Empty;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        // Attempt to write single-quoted string
        writer.WriteRawValue($"'{value.Replace("'", "\\'")}'", true);
    }
}
