using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Rizzy.Components.Swap.Services;

namespace Rizzy.Components;

/// <summary>
/// Razor component that fully renders any content from HtmxSwapService
/// </summary>
public class HtmxSwapContent : ComponentBase
{
	private RenderFragment? _fragment;

    [Inject] public IHtmxSwapService HtmxSwapService { get; set; } = default!;

    protected override void OnInitialized()
    {
	    _fragment = HtmxSwapService.RenderToFragment();
        HtmxSwapService.Clear();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (_fragment != null)
	        builder.AddContent(0, _fragment);
    }
}

