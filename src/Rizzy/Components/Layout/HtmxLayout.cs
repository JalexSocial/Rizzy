using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Rizzy.Http;

namespace Rizzy.Components;

public sealed class HtmxLayout<T> : LayoutComponentBase where T : LayoutComponentBase
{
    [CascadingParameter]
    public RzViewContext? ViewContext { get; set; }

    [Parameter]
    public bool IsRootComponent { get; set; } = false;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (ViewContext?.Htmx.Request?.IsHtmx == true)
        {
            ViewContext?.HttpContext.Response.Headers.TryAdd("Vary", HtmxRequestHeaderNames.HtmxRequest);

            // If streaming is in use then content needs to be wrapped in a minimal layout
            if (IsRootComponent)
            {
                builder.OpenComponent<MinimalLayout>(0);
            }
            else
            {
                builder.OpenComponent<EmptyLayout>(0);
            }
        }
        else
        {
            builder.OpenComponent<T>(0);
        }

        builder.AddComponentParameter(1, "Body", Body);
        builder.CloseComponent();
    }
}