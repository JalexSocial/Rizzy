using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Rizzy.Components.Swap.Services;

namespace Rizzy.Components;

/// <summary>
/// Razor component that fully renders any content from HtmxSwapService
/// </summary>
public class HtmxSwapContent : ComponentBase
{
    [Inject] public IHtmxSwapService HtmxSwapService { get; set; } = default!;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AddContent(0, HtmxSwapService.RenderToFragment());
    }
}

