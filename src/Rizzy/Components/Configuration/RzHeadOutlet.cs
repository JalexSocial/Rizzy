using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Rizzy;

/// <summary>
/// Use in place of Blazor's <see cref="HeadOutlet"/> in your root layout's <c>&lt;head&gt;</c>, before
/// the htmx <c>&lt;script&gt;</c>. It renders <see cref="HeadOutlet"/> (page titles and
/// <see cref="HeadContent"/>) followed by Rizzy's <c>&lt;meta name="htmx-config"&gt;</c> tag
/// (<see cref="HtmxConfigHeadOutlet"/>), which carries the htmx settings and the antiforgery token.
/// </summary>
/// <remarks>
/// The config tag is rendered directly rather than through <see cref="HeadContent"/>: Blazor shows
/// only the most recently rendered <see cref="HeadContent"/>, so a page with its own
/// <see cref="HeadContent"/> would otherwise replace it (RIZZY-036). htmx reads the tag once, when
/// its script loads, so the outlet must come before the htmx script.
/// </remarks>
public sealed class RzHeadOutlet : ComponentBase
{
    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<HeadOutlet>(0);
        builder.CloseComponent();
        builder.OpenComponent<HtmxConfigHeadOutlet>(1);
        builder.CloseComponent();
    }
}
