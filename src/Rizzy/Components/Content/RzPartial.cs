using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Rizzy.Components.Swap;
using Rizzy.Framework.Mvc;
using Rizzy.Framework.Services;

namespace Rizzy.Components.Content;

public class RzPartial : ComponentBase
{
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, Type?> _layoutAttributeCache = new();

    private class EmptyLayout : LayoutComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "html");
            builder.OpenElement(1, "body");
            builder.AddContent(2, Body);
            builder.CloseElement();
            builder.CloseElement();
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
	    ContextWrapper.CreateCascadingValue(builder, ViewContext.HttpContext, (builder2) =>
	    {
		    ContextWrapper.CreateCascadingValue(builder2, ViewContext, (builder3) =>
		    {
			    builder3.OpenComponent<LayoutView>(4);
			    builder3.AddComponentParameter(5, "Layout", typeof(EmptyLayout));
			    builder3.AddAttribute(6, "ChildContent", (RenderFragment)((builder4) =>
			    {
				    builder4.OpenComponent<DynamicComponent>(7);
				    builder4.AddComponentParameter(8, "Type", RuntimeHelpers.TypeCheck<System.Type>(ComponentType));
				    builder4.AddComponentParameter(9, "Parameters", RuntimeHelpers.TypeCheck<System.Collections.Generic.IDictionary<string, object?>>(ComponentParameters));
				    builder4.CloseComponent();
			    }));
			    builder3.CloseComponent();
			    builder3.OpenComponent<HtmxSwapContent>(15);
			    builder3.CloseComponent();
		    });
		});
    }

    [Parameter, EditorRequired] public Type ComponentType { get; set; } = default!;

    [Parameter, EditorRequired] public Dictionary<string, object?> ComponentParameters { get; set; } = default!;

    [Parameter, EditorRequired] public RzViewContext ViewContext { get; set; } = default!;
}