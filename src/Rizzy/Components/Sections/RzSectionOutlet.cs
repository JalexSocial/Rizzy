using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;

namespace Rizzy.Components;

/// <summary>
/// Renders content provided by <see cref="RzSectionContent"/> components with matching <see cref="SectionId"/>s.
/// This class acts as a proxy to the original <see cref="SectionOutlet"/>, forwarding all parameters directly to it
/// and rendering its output with no additional markup.
/// </summary>
public class RzSectionOutlet : ComponentBase
{
    /// <summary>
    /// Gets or sets the <see cref="string"/> ID that determines which <see cref="RzSectionContent"/> instances will provide
    /// content to this instance.
    /// </summary>
    [Parameter] public string? SectionName { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="object"/> ID that determines which <see cref="RzSectionContent"/> instances will provide
    /// content to this instance.
    /// </summary>
    [Parameter] public object? SectionId { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<SectionOutlet>(0);
        builder.AddAttribute(1, nameof(SectionName), SectionName);
        builder.AddAttribute(2, nameof(SectionId), SectionId);
        builder.CloseComponent();
    }
}