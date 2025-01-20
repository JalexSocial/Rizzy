using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Options;
using Rizzy.Configuration;
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

    public static void CreateCascadingValue<TValue>(RenderTreeBuilder builder, TValue value, RenderFragment fragment)
    {
        builder.OpenComponent<CascadingValue<TValue>>(0);
        builder.AddComponentParameter(1, "Value", value);
        builder.AddComponentParameter(2, "ChildContent", fragment);
        builder.CloseComponent();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {

        builder.OpenComponent(4, RizzyConfig.Value.RootComponent ?? typeof(EmptyRootComponent));
        builder.AddAttribute(5, "ChildContent", (RenderFragment)(builder3 =>
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
        builder.OpenComponent<HtmxSwapContent>(15);
        builder.CloseComponent();
        builder.CloseComponent();

    }

    [Parameter, EditorRequired] public required Type ComponentType { get; set; } = default!;

    [Parameter, EditorRequired] public required Dictionary<string, object?> ComponentParameters { get; set; } = default!;

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

