using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Rizzy.Htmx;

namespace Rizzy;

/// <summary>
/// Enables rendering an HTML <c>&lt;title&gt;</c> to the document head. This component currently is not operational in SSR mode
/// due to existing Blazor issues.  See https://github.com/dotnet/aspnetcore/issues/50268
/// </summary>
/// <remarks>
/// Set the title with <see cref="Text"/> or <see cref="ChildContent"/>. On .NET 11, inside a
/// <c>CacheView</c> use <see cref="Text"/>: the component re-renders on every request, so the title
/// is set even when the surrounding markup comes from the cache. <see cref="ChildContent"/> cannot
/// be used there, and the framework throws a descriptive exception if it is.
/// </remarks>
[RizzyParameterize]
#if NET11_0_OR_GREATER
[CacheBehavior(CacheBehavior.Rerender)]
#endif
public sealed partial class RzPageTitle : ComponentBase
{
    [CascadingParameter]
    public HttpContext? HttpContext { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered as the document title.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the document title as plain text. Used when <see cref="ChildContent"/> is not set.
    /// </summary>
    [Parameter]
    public string? Text { get; set; }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (HttpContext?.Request.Htmx().IsPartial == true)
        {
            builder.OpenElement(0, "title");
            if (ChildContent is not null)
                builder.AddContent(1, ChildContent);
            else
                builder.AddContent(2, Text);
            builder.CloseElement();
        }
        else
        {
            builder.OpenComponent<PageTitle>(0);
            builder.AddComponentParameter(1, nameof(PageTitle.ChildContent), ChildContent ?? RenderText);
            builder.CloseComponent();
        }
    }

    private void RenderText(RenderTreeBuilder builder) => builder.AddContent(0, Text);
}
