using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Rizzy.Framework.Mvc;
using Rizzy.Framework.Services;
using Rizzy.Http;

namespace Rizzy.Components.Layout;

public class HtmxLayout<T> : LayoutComponentBase where T : LayoutComponentBase
{
    internal class BaseLayout : LayoutComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(0, Body);
        }
    }

    [CascadingParameter]
    public RzViewContext? ViewContext { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (ViewContext?.Htmx.Request?.IsHtmx == true)
        {
            ViewContext?.HttpContext.Response.Headers.TryAdd("Vary", HtmxRequestHeaderNames.HtmxRequest);
            builder.OpenComponent<BaseLayout>(0);
        }
        else
        {
            builder.OpenComponent<T>(0);
        }

        builder.AddComponentParameter(1, "Body", Body);
        builder.CloseComponent();
    }
}
