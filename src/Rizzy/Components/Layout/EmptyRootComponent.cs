using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Rizzy;

internal class EmptyRootComponent : ComponentBase
{
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "html");
        builder.OpenElement(1, "body");
        builder.AddContent(2, ChildContent);
        builder.CloseElement();
        builder.CloseElement();
    }
}
