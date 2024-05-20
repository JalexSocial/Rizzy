using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Rizzy.Configuration.Htmx.Builder;
using Rizzy.Configuration.Htmx.Enum;

namespace Rizzy.Components;

/// <summary>
/// A Blazor component that enables swapping HTML content dynamically based on specified parameters through Htmx.
/// </summary>
public class HtmxSwappable : ComponentBase
{
    private string _swapParam = string.Empty;

    /// <summary>
    /// Gets or sets the target DOM element ID where the component should be rendered.
    /// This parameter is required.
    /// </summary>
    [Parameter, EditorRequired]
    public string TargetId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the swap style to be applied when the content is swapped.
    /// The default swap style is OuterHTML.
    /// </summary>
    [Parameter]
    public SwapStyle SwapStyle { get; set; } = SwapStyle.outerHTML;

    /// <summary>
    /// Gets or sets the CSS selector for the content swap. This is optional.
    /// </summary>
    [Parameter]
    public string Selector { get; set; } = default!;

    /// <summary>
    /// Gets or sets the child content to be rendered within the component.
    /// This parameter is required.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Prepares the swap parameters based on the current properties' values.
    /// This method is called when the parameters are set.
    /// </summary>
    protected override void OnParametersSet()
    {
        var style = SwapStyle.ToHtmxString();

        if (!string.IsNullOrEmpty(Selector))
            _swapParam = $"{style}:{Selector}";
        else
            _swapParam = style;
    }

    /// <summary>
    /// Builds the render tree for the component. This method is responsible for rendering
    /// the component based on the swap parameters and child content.
    /// </summary>
    /// <param name="builder">The <see cref="RenderTreeBuilder"/> used to construct the component's render tree.</param>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "id", TargetId);
        builder.AddAttribute(2, "hx-swap-oob", _swapParam);
        builder.AddContent(3, ChildContent);
        builder.CloseElement();
    }
}

