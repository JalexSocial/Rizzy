using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rizzy.Components.Form.Helpers;
using System.Collections.ObjectModel;

namespace Rizzy.Components;

/// <summary>
/// A component that wraps the HTML file input element and supplies a Stream for each file's contents.
/// </summary>
public class RzInputFile : InputFile
{
	[CascadingParameter]
	private Dictionary<FieldIdentifier, string>? FieldMapping { get; set; }

    [Parameter]
	public string Id { get; set; } = string.Empty;

	protected override void OnParametersSet()
	{
		base.OnParametersSet();

		if (FieldMapping is null)
			throw new InvalidOperationException($"{nameof(RzInputFile)} must be enclosed within an {nameof(RzEditForm)}.");

        // No validation

        if (!string.IsNullOrEmpty(Id))
        {
	        var attrib = AdditionalAttributes is null ? new Dictionary<string, object>() : new Dictionary<string, object>(AdditionalAttributes);
	        attrib.TryAdd("id", Id);

	        AdditionalAttributes = new ReadOnlyDictionary<string, object>(attrib);
        }
    }
}