using System.Text.Json.Serialization;

namespace Rizzy.Htmx.Serialization;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DictionaryKeyPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    UseStringEnumConverter = true,
    GenerationMode = JsonSourceGenerationMode.Default,
    Converters = [
        typeof(TimespanMillisecondJsonConverter),
        typeof(SwapStyleEnumConverter),
        typeof(HtmxHistoryModeJsonConverter),
    ])]
[JsonSerializable(typeof(HtmxConfig))]
internal sealed partial class HtmxJsonSerializerContext : JsonSerializerContext
{
}
