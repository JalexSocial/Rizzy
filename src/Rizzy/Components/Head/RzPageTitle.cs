using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;
using Rizzy.Framework.Services;

namespace Rizzy.Components.Head;

/// <summary>
/// Enables rendering an HTML <c>&lt;title&gt;</c> to a <see cref="RzHeadOutlet"/> component.
/// </summary>
public sealed class RzPageTitle : ComponentBase
{
    [Inject] public RzViewContext ViewContext { get; set; } = default!;

    /// <summary>
    /// Gets or sets the content to be rendered as the document title.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (ViewContext.Htmx.Request is { IsHtmx: true, IsBoosted: false })
        {
            builder.OpenComponent<SectionContent>(0);
            builder.AddComponentParameter(1, nameof(SectionContent.SectionId), RzHeadOutlet.TitleSectionId);
            builder.AddComponentParameter(2, nameof(SectionContent.ChildContent), (RenderFragment)BuildTitleRenderTree);
            builder.CloseComponent();
        }
    }

    private void BuildTitleRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "title");
        builder.AddContent(1, ChildContent);
        builder.CloseElement();
    }
}
