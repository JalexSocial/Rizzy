using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rizzy.Serialization;

internal sealed class JsonCamelCaseStringEnumConverter<TEnum> : JsonStringEnumConverter<TEnum>
    where TEnum : struct, Enum
{
    public JsonCamelCaseStringEnumConverter()
        : base(JsonNamingPolicy.CamelCase, allowIntegerValues: false)
    {
    }
}
