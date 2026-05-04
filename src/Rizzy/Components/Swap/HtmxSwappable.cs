using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Options;
using Rizzy.Htmx;

namespace Rizzy;

[RizzyParameterize] 
public partial class HtmxSwappable : ComponentBase
{
    private string _swapParam = string.Empty;

    [Inject]
    private IOptionsSnapshot<HtmxConfig> HtmxOptions { get; set; } = default!;

    [Parameter, EditorRequired]
    public string TargetId { get; set; } = default!;

    [Parameter]
    public SwapStyle SwapStyle { get; set; } = SwapStyle.outerHTML;

    [Parameter]
    public string Selector { get; set; } = default!;

    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    protected override void OnParametersSet()
    {
        _swapParam = SwapStyle.ToHtmxString();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "hx-partial");

        var target = !string.IsNullOrEmpty(Selector) ? Selector : $"#{TargetId}";
        var prefix = HtmxOptions.Value.Prefix;

        builder.AddAttribute(1, HtmxAttributeName.Resolve("hx-target", prefix), target);
        builder.AddAttribute(2, HtmxAttributeName.Resolve("hx-swap", prefix), _swapParam);
        builder.AddContent(3, ChildContent);
        builder.CloseElement();
    }
}
