using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rizzy.Htmx;
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
            Constants.SwapStyles.InnerMorph => SwapStyle.innerMorph,
            Constants.SwapStyles.OuterMorph => SwapStyle.outerMorph,
            Constants.SwapStyles.TextContent => SwapStyle.textContent,
            Constants.SwapStyles.Before => SwapStyle.before,
            Constants.SwapStyles.After => SwapStyle.after,
            Constants.SwapStyles.Prepend => SwapStyle.prepend,
            Constants.SwapStyles.Append => SwapStyle.append,
            _ => throw new SwitchExpressionException(value)
        };

        return style;
    }

    public override void Write(Utf8JsonWriter writer, SwapStyle value, JsonSerializerOptions options)
    {
        writer?.WriteStringValue(value.ToHtmxString());
    }
}
