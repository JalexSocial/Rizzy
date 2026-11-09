using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Rizzy;

/// <summary>
/// Razor component that fully renders any content from HtmxSwapService
/// </summary>
public class HtmxSwapContent : ComponentBase, IDisposable
{
    [Inject] public IHtmxSwapService HtmxSwapService { get; set; } = default!;

    protected override void OnInitialized()
    {
        HtmxSwapService.ContentItemsUpdated += OnContentItemsUpdated;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (HtmxSwapService.ContentAvailable)
            builder.AddContent(0, HtmxSwapService.RenderToFragment());
    }

    private void OnContentItemsUpdated(object? sender, EventArgs args) => InvokeAsync(StateHasChanged);

    /// <inheritdoc />
    public void Dispose()
    {
        HtmxSwapService.ContentItemsUpdated -= OnContentItemsUpdated;
        GC.SuppressFinalize(this);
    }
}
