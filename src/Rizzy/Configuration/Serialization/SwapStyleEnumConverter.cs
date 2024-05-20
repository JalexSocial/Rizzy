using Rizzy.Configuration.Htmx.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Rizzy.Configuration.Htmx.Builder;
using System.Text.Json.Serialization;

namespace Rizzy.Configuration.Serialization;
internal sealed class SwapStyleEnumConverter : JsonConverter<SwapStyle>
{
    public override SwapStyle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        var style = value switch
        {
            null => SwapStyle.Default,
            Constants.SwapStyles.Default => SwapStyle.Default,
            Constants.SwapStyles.InnerHTML => SwapStyle.innerHTML,
            Constants.SwapStyles.OuterHTML => SwapStyle.outerHTML,
            Constants.SwapStyles.BeforeBegin => SwapStyle.beforebegin,
            Constants.SwapStyles.AfterBegin => SwapStyle.afterbegin,
            Constants.SwapStyles.BeforeEnd => SwapStyle.beforeend,
            Constants.SwapStyles.AfterEnd => SwapStyle.afterend,
            Constants.SwapStyles.Delete => SwapStyle.delete,
            Constants.SwapStyles.None => SwapStyle.none,
            _ => throw new SwitchExpressionException(value)
        };

        return style;
    }

    public override void Write(Utf8JsonWriter writer, SwapStyle value, JsonSerializerOptions options)
    {
        writer?.WriteStringValue(value.ToHtmxString());
    }
}
