using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;

namespace Rizzy;


/// <summary>
/// This class acts as a proxy to the original <see cref="SectionContent"/>, forwarding all parameters directly to it
/// and rendering its output with no additional markup.
/// </summary>
public class RzSectionContent : ComponentBase
{
    /// <summary>
    /// Gets or sets the <see cref="string"/> ID that determines which <see cref="SectionOutlet"/> instance will render
    /// the content of this instance.
    /// </summary>
    [Parameter] public string? SectionName { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="object"/> ID that determines which <see cref="SectionOutlet"/> instance will render
    /// the content of this instance.
    /// </summary>
    [Parameter] public object? SectionId { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered in corresponding <see cref="SectionOutlet"/> instances.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<SectionContent>(0);
        builder.AddAttribute(1, nameof(SectionName), SectionName);
        builder.AddAttribute(2, nameof(SectionId), SectionId);
        builder.AddAttribute(3, nameof(ChildContent), ChildContent);
        builder.CloseComponent();
    }
}
