using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;

namespace Rizzy.Components;

public partial class RzPartial : ComponentBase
{
    public static void CreateCascadingValue<TValue>(RenderTreeBuilder builder, TValue value, RenderFragment fragment)
    {
        builder.OpenComponent<CascadingValue<TValue>>(0);
        builder.AddComponentParameter(1, "Value", value);
        builder.AddComponentParameter(2, "ChildContent", fragment);
        builder.CloseComponent();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
	    builder.OpenComponent<LayoutView>(4);
        builder.AddComponentParameter(5, "Layout", typeof(EmptyLayout));
        builder.AddAttribute(6, "ChildContent", (RenderFragment)((builder2) =>
        {
            builder2.OpenComponent<DynamicComponent>(7);
            builder2.AddComponentParameter(8, "Type", RuntimeHelpers.TypeCheck<System.Type>(ComponentType));
            builder2.AddComponentParameter(9, "Parameters", RuntimeHelpers.TypeCheck<System.Collections.Generic.IDictionary<string, object?>>(ComponentParameters));
            builder2.CloseComponent();
        }));
        builder.CloseComponent();
        builder.OpenComponent<HtmxSwapContent>(15);
        builder.CloseComponent();
    }

    [Parameter, EditorRequired] public required Type ComponentType { get; set; } = default!;

    [Parameter, EditorRequired] public required Dictionary<string, object?> ComponentParameters { get; set; } = default!;
}