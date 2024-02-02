using Microsoft.AspNetCore.Components;
using Rizzy.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rizzy.Configuration;

namespace Rizzy.Components;

public class RzPage : ComponentBase
{
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, Type?> _layoutAttributeCache = new();
    private Type? _layout = null;

    internal static class ViewContextWrapper
    {
        public static void CreateCascadingValue<TValue>(RenderTreeBuilder builder, TValue value, RenderFragment fragment)
        {
            builder.OpenComponent<CascadingValue<TValue>>(0);
            builder.AddComponentParameter(1, "Value", value);
            builder.AddComponentParameter(2, "ChildContent", fragment);
            //builder.AddComponentParameter(3, "Name", "ViewContext");
            builder.CloseComponent();
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        ViewContextWrapper.CreateCascadingValue(builder, ViewContext, (builder2) => {

            if (_layout != null)
            {
                builder2.OpenComponent<LayoutView>(4);
                builder2.AddComponentParameter(5, "Layout", RuntimeHelpers.TypeCheck<System.Type>(_layout));
                builder2.AddAttribute(6, "ChildContent", (RenderFragment)((builder3) => {
                    builder3.OpenComponent<DynamicComponent>(7);
                    builder3.AddComponentParameter(8, "Type", RuntimeHelpers.TypeCheck<System.Type>(ComponentType));
                    builder3.AddComponentParameter(9, "Parameters", RuntimeHelpers.TypeCheck<System.Collections.Generic.IDictionary<string, object?>>(ComponentParameters));
                    builder3.CloseComponent();
                }));
                builder2.CloseComponent();
            }
            else
            {
                builder2.OpenComponent<DynamicComponent>(10);
                builder2.AddComponentParameter(11, "Type", RuntimeHelpers.TypeCheck<System.Type>(ComponentType));
                builder2.AddComponentParameter(12, "Parameters", RuntimeHelpers.TypeCheck<System.Collections.Generic.IDictionary<string, object?>>(ComponentParameters));
                builder2.CloseComponent();
            }
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
