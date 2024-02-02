using System.Text.Json.Serialization;
using Rizzy.Configuration.Htmx;
using Rizzy.Configuration.Htmx.Enum;

namespace Rizzy.Configuration.Serialization;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    UseStringEnumConverter = true,
    GenerationMode = JsonSourceGenerationMode.Default,
    Converters = [typeof(TimespanMillisecondJsonConverter), typeof(JsonCamelCaseStringEnumConverter<SwapStyle>), typeof(JsonCamelCaseStringEnumConverter<ScrollBehavior>)])]
[JsonSerializable(typeof(HtmxConfig))]
internal sealed partial class HtmxConfigJsonSerializerContext : JsonSerializerContext
{
}
