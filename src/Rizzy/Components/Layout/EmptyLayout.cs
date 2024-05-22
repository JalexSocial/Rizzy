using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Rizzy.Components;

internal class EmptyLayout : LayoutComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AddContent(0, Body);
    }
}