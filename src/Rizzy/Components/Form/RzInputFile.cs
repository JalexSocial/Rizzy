using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.ObjectModel;

namespace Rizzy;

/// <summary>
/// A component that wraps the HTML file input element and supplies a Stream for each file's contents.
/// </summary>
public class RzInputFile : InputFile
{
    /// <summary>
    /// Gets or sets the ID of the input element.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the cascading parameter for the EditContext.
    /// </summary>
    [CascadingParameter] EditContext EditContext { get; set; } = default!;

    /// <summary>
    /// Method invoked when the component has received parameters from its parent in the render tree.
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (EditContext is null)
            throw new InvalidOperationException($"{nameof(RzInputFile)} must be enclosed within an {nameof(EditForm)}.");

        // No validation

        if (!string.IsNullOrEmpty(Id))
        {
            var attrib = AdditionalAttributes is null ? new Dictionary<string, object>() : new Dictionary<string, object>(AdditionalAttributes);
            attrib.TryAdd("id", Id);

            AdditionalAttributes = new ReadOnlyDictionary<string, object>(attrib);
        }
    }
}
