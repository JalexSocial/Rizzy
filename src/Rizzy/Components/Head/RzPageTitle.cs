using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Rizzy.Components;

/// <summary>
/// Enables rendering an HTML <c>&lt;title&gt;</c> to a <see cref="RzHeadOutlet"/> component. This component currently is not operational in SSR mode
/// due to existing Blazor issues.  See https://github.com/dotnet/aspnetcore/issues/50268
/// </summary>
public sealed class RzPageTitle : ComponentBase
{
    [CascadingParameter] 
    public HttpContext? HttpContext { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered as the document title.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (HttpContext?.Request.IsHtmx() == true)
        {
            builder.OpenComponent<SectionContent>(0);
            builder.AddComponentParameter(1, nameof(SectionContent.SectionId), RzHeadOutlet.TitleSectionId);
            builder.AddComponentParameter(2, nameof(SectionContent.ChildContent), (RenderFragment)BuildTitleRenderTree);
            builder.CloseComponent();
        }
        else
        {
            BuildTitleRenderTree(builder);
        }
    }

    private void BuildTitleRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "title");
        builder.AddContent(1, ChildContent);
        builder.CloseElement();
    }
}
