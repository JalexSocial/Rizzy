using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rizzy.Htmx.Serialization;

public sealed class HtmxHistoryModeJsonConverter : JsonConverter<HtmxHistoryMode>
{
    public override HtmxHistoryMode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.True) return HtmxHistoryMode.Enabled;
        if (reader.TokenType == JsonTokenType.False) return HtmxHistoryMode.Disabled;
        if (reader.TokenType == JsonTokenType.String && string.Equals(reader.GetString(), "reload", StringComparison.OrdinalIgnoreCase))
            return HtmxHistoryMode.Reload;

        throw new JsonException("Expected true, false, or \"reload\" for htmx history config.");
    }

    public override void Write(Utf8JsonWriter writer, HtmxHistoryMode value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case HtmxHistoryMode.Enabled: writer.WriteBooleanValue(true); break;
            case HtmxHistoryMode.Disabled: writer.WriteBooleanValue(false); break;
            case HtmxHistoryMode.Reload: writer.WriteStringValue("reload"); break;
            default: throw new JsonException($"Unsupported history mode: {value}");
        }
    }
}
