using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Http;

namespace Rizzy.Components;

public sealed class HtmxApp<T> : ComponentBase where T : LayoutComponentBase
{
    [CascadingParameter]
    public HttpContext? Context { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<HtmxLayout<T>>(0);
        builder.AddAttribute(1, "IsRootComponent", true);
        builder.AddAttribute(2, "Body", (RenderFragment)(builder2 =>
        {
            builder2.AddContent(3, ChildContent);
        }));
        builder.CloseComponent();
    }
}
