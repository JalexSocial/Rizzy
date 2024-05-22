using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Rizzy.Components;

internal class MinimalLayout : LayoutComponentBase
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
