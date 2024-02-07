using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;
using Rizzy.Components.Swap.Services;

namespace Rizzy.Components.Swap;

public class HtmxSwapContent : ComponentBase
{
    [Inject] public IHtmxSwapService HtmxSwapService { get; set; } = default!;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AddContent(0, HtmxSwapService.RenderToFragment());
    }
}

