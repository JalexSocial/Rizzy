using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Rizzy;

/// <summary>
/// A Razor component used to render a partial view without a layout.
/// </summary>
public partial class RzPartial : ComponentBase
{
    /// <summary>
    /// Creates a cascading value around a given render fragment.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to cascade.</typeparam>
    /// <param name="builder">The <see cref="RenderTreeBuilder"/> to use.</param>
    /// <param name="value">The value to supply.</param>
    /// <param name="fragment">The child content to render inside the cascading value.</param>
    public static void CreateCascadingValue<TValue>(RenderTreeBuilder builder, TValue value, RenderFragment fragment)
    {
        builder.OpenComponent<CascadingValue<TValue>>(0);
        builder.AddComponentParameter(1, "Value", value);
        builder.AddComponentParameter(2, "ChildContent", fragment);
        builder.CloseComponent();
    }

    /// <summary>
    /// Builds the render tree for this partial component.
    /// </summary>
    /// <param name="builder">The <see cref="RenderTreeBuilder"/> to receive the render output.</param>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        CreateCascadingValue(builder, ModelState, builder2 =>
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
	        builder2.OpenComponent<HtmxSwapContent>(15);
	        builder2.CloseComponent();
        });
    }

    /// <summary>
    /// Gets or sets the type of the Razor component to render.
    /// </summary>
    [Parameter, EditorRequired]
    public required Type ComponentType { get; set; } = default!;

    /// <summary>
    /// Gets or sets any component parameters to be passed dynamically.
    /// </summary>
    [Parameter, EditorRequired]
    public required Dictionary<string, object?> ComponentParameters { get; set; } = default!;

    /// <summary>
    /// Optional ModelState provided by MVC
    /// </summary>
    [Parameter]
    public ModelStateDictionary? ModelState { get; set; }

}
