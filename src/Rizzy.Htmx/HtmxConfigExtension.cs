using Rizzy.Htmx.Serialization;
using System.Text.Json;

namespace Rizzy.Htmx;

public static class HtmxConfigExtension
{
    public static string Serialize(this HtmxConfig config)
    {
        return JsonSerializer.Serialize(config, HtmxJsonSerializerContext.Default.HtmxConfig);
    }
}
