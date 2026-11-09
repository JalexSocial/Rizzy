using System.Text.Json.Serialization;

namespace Rizzy.Htmx.Serialization;

/// <summary>
/// Serializer context for <c>HX-Location</c> options. Unlike the config context it applies no
/// dictionary key policy, so header and value names are sent exactly as given.
/// </summary>
[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(LocationTarget))]
[JsonSerializable(typeof(AjaxContext))]
internal sealed partial class HtmxAjaxJsonSerializerContext : JsonSerializerContext
{
}
