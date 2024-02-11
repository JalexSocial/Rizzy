using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rizzy.Components.Layout;
using Rizzy.Configuration;
using Rizzy.Framework.Mvc;
using System.Reflection;
using Rizzy.Components.Swap;
using Rizzy.Framework.Services;

namespace Rizzy.Components.Content;

public class RzPage : ComponentBase
{
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, Type?> _layoutAttributeCache = new();
    private Type? _layout = null;

    [Inject] public IOptions<RizzyConfig> RizzyConfig { get; set; } = default!;

    private static class ContextWrapper
    {
        public static void CreateCascadingValue<TValue>(RenderTreeBuilder builder, TValue value, RenderFragment fragment)
        {
            builder.OpenComponent<CascadingValue<TValue>>(0);
            builder.AddComponentParameter(1, "Value", value);
            builder.AddComponentParameter(2, "ChildContent", fragment);
            builder.CloseComponent();
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
	    ContextWrapper.CreateCascadingValue(builder, ViewContext.HttpContext, (builder2) =>
	    {
		    ContextWrapper.CreateCascadingValue(builder2, ViewContext, (builder3) =>
		    {
			    builder3.OpenComponent(4, RizzyConfig.Value.RootComponent ?? typeof(EmptyRootComponent));
			    builder3.AddAttribute(5, "ChildContent", (RenderFragment)(builder4 =>
			    {
				    if (_layout != null)
				    {
					    builder4.OpenComponent<LayoutView>(6);
					    builder4.AddComponentParameter(7, "Layout", RuntimeHelpers.TypeCheck<System.Type>(_layout));
					    builder4.AddAttribute(8, "ChildContent", (RenderFragment)((builder5) =>
					    {
						    builder5.OpenComponent<DynamicComponent>(9);
						    builder5.AddComponentParameter(10, "Type", RuntimeHelpers.TypeCheck<System.Type>(ComponentType));
						    builder5.AddComponentParameter(11, "Parameters", RuntimeHelpers.TypeCheck<System.Collections.Generic.IDictionary<string, object?>>(ComponentParameters));
						    builder5.CloseComponent();
					    }));
					    builder4.CloseComponent();
				    }
				    else
				    {
					    builder4.OpenComponent<DynamicComponent>(12);
					    builder4.AddComponentParameter(13, "Type", RuntimeHelpers.TypeCheck<System.Type>(ComponentType));
					    builder4.AddComponentParameter(14, "Parameters", RuntimeHelpers.TypeCheck<System.Collections.Generic.IDictionary<string, object?>>(ComponentParameters));
					    builder4.CloseComponent();
				    }
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

    protected override void OnParametersSet()
    {
        if (!_layoutAttributeCache.TryGetValue(ComponentType, out _layout))
        {
            _layout = ComponentType.GetCustomAttribute<LayoutAttribute>()?.LayoutType;
            _layoutAttributeCache.TryAdd(ComponentType, _layout);
        }

        if (_layout == null)
        {
            var config = ViewContext?.HttpContext.RequestServices.GetRequiredService<IOptions<RizzyConfig>>();

            if (config?.Value.DefaultLayout != null)
                _layout = config?.Value.DefaultLayout;
        }
    }
}

