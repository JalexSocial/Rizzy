﻿using Microsoft.AspNetCore.Components;
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
        if (HttpContext?.Request.IsHtmx() == true)
        {
            HttpContext?.Response.Headers.TryAdd("Vary", HtmxRequestHeaderNames.HtmxRequest);

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