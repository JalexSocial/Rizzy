using Rizzy.Configuration.Htmx;
using Rizzy.Configuration.Htmx.Enum;
using Rizzy.Http.Models;
using System.Text.Json.Serialization;

namespace Rizzy.Configuration.Serialization;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DictionaryKeyPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    UseStringEnumConverter = true,
    GenerationMode = JsonSourceGenerationMode.Default,
    Converters = [
        typeof(TimespanMillisecondJsonConverter),
        typeof(SwapStyleEnumConverter),
        typeof(JsonCamelCaseStringEnumConverter<ScrollBehavior>),
    ])]
[JsonSerializable(typeof(HtmxConfig))]
[JsonSerializable(typeof(LocationTarget))]
[JsonSerializable(typeof(AjaxContext))]
internal sealed partial class HtmxJsonSerializerContext : JsonSerializerContext
{
}
