using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rizzy.Configuration;
using Rizzy.Framework.Services;
using System.Reflection;

namespace Rizzy.Components;

/// <summary>
/// Page component container that provides a ViewContext cascading value
/// </summary>
public class RzPage : ComponentBase
{
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, Type?> _layoutAttributeCache = new();
    private Type? _layout = null;

    [Inject] public IOptions<RizzyConfig> RizzyConfig { get; set; } = default!;

    private static class ViewContextWrapper
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
        ViewContextWrapper.CreateCascadingValue(builder, ViewContext, (builder2) =>
        {
            builder2.OpenComponent(4, RizzyConfig.Value.RootComponent ?? typeof(EmptyRootComponent));
            builder2.AddAttribute(5, "ChildContent", (RenderFragment)(builder3 =>
            {
                if (_layout != null)
                {
                    builder3.OpenComponent<LayoutView>(6);
                    builder3.AddComponentParameter(7, "Layout", RuntimeHelpers.TypeCheck<System.Type>(_layout));
                    builder3.AddAttribute(8, "ChildContent", (RenderFragment)((builder4) =>
                    {
                        builder4.OpenComponent<DynamicComponent>(9);
                        builder4.AddComponentParameter(10, "Type", RuntimeHelpers.TypeCheck<System.Type>(ComponentType));
                        builder4.AddComponentParameter(11, "Parameters", RuntimeHelpers.TypeCheck<System.Collections.Generic.IDictionary<string, object?>>(ComponentParameters));
                        builder4.CloseComponent();
                    }));
                    builder3.CloseComponent();
                }
                else
                {
                    builder3.OpenComponent<DynamicComponent>(12);
                    builder3.AddComponentParameter(13, "Type", RuntimeHelpers.TypeCheck<System.Type>(ComponentType));
                    builder3.AddComponentParameter(14, "Parameters", RuntimeHelpers.TypeCheck<System.Collections.Generic.IDictionary<string, object?>>(ComponentParameters));
                    builder3.CloseComponent();
                }
            }));
            builder2.CloseComponent();
            builder2.OpenComponent<HtmxSwapContent>(15);
            builder2.CloseComponent();
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
            var config = RizzyConfig;

            if (config?.Value.DefaultLayout != null)
                _layout = config?.Value.DefaultLayout;
        }
    }
}

