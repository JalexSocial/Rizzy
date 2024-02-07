using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Rizzy.Components.Swap;
using Rizzy.Framework.Mvc;

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
            builder2.OpenComponent<LayoutView>(4);
            builder2.AddComponentParameter(5, "Layout", typeof(EmptyLayout));
            builder2.AddAttribute(6, "ChildContent", (RenderFragment)((builder3) =>
            {
                builder3.OpenComponent<DynamicComponent>(7);
                builder3.AddComponentParameter(8, "Type", RuntimeHelpers.TypeCheck<System.Type>(ComponentType));
                builder3.AddComponentParameter(9, "Parameters", RuntimeHelpers.TypeCheck<System.Collections.Generic.IDictionary<string, object?>>(ComponentParameters));
                builder3.CloseComponent();
            }));
            builder2.CloseComponent();
            builder2.OpenComponent<RzSwapContent>(15);
            builder2.CloseComponent();
        });
    }

    [Parameter, EditorRequired] public Type ComponentType { get; set; } = default!;

    [Parameter, EditorRequired] public Dictionary<string, object?> ComponentParameters { get; set; } = default!;

    [Parameter, EditorRequired] public RzViewContext ViewContext { get; set; } = default!;
}