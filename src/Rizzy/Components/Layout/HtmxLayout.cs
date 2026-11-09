using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Http;
using Rizzy.Htmx;

namespace Rizzy;

public sealed class HtmxLayout<T> : LayoutComponentBase where T : LayoutComponentBase
{
    [CascadingParameter]
    public HttpContext? HttpContext { get; set; }

    [Parameter]
    public bool IsRootComponent { get; set; } = false;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // Full pages and fragments share URLs, so every response must vary on the htmx headers.
        HttpContext?.Response.VaryByHtmxHeaders();

        // Only partial htmx requests get a fragment. HX-Request-Type: full (boosted body swaps,
        // hx-select) and history restores re-fetch the whole page and need the real layout.
        if (HttpContext?.Request.Htmx().IsPartial == true)
        {
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