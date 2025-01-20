using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Rizzy.Htmx.Serialization;

namespace Rizzy.Htmx;

public static class HtmxConfigExtension
{
	public static string Serialize(this HtmxConfig config)
	{
        return JsonSerializer.Serialize(config, HtmxJsonSerializerContext.Default.HtmxConfig);
    }
}
