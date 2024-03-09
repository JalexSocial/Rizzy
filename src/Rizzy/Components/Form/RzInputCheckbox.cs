using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rizzy.Components.Form.Helpers;

namespace Rizzy.Components;

/// <summary>
/// An input component for editing Boolean values.
/// </summary>
public class RzInputCheckbox : InputCheckbox
{
    [Inject]
    public DataAnnotationsProcessor DataAnnotationsProcessor { get; set; } = default!;

    [CascadingParameter]
    private Dictionary<FieldIdentifier, string>? FieldMapping { get; set; }

    [Parameter]
    public string Id { get; set; } = string.Empty;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (FieldMapping is null)
            throw new InvalidOperationException($"{nameof(RzInputCheckbox)} must be enclosed within an {nameof(RzEditForm)}.");

        // No validation

        // If id doesn't exist then attempt to create one
        if (string.IsNullOrEmpty(Id))
        {
	        Id = IdProvider.CreateSanitizedId(NameAttributeValue);
        }

        var attrib = AdditionalAttributes is null ? new Dictionary<string, object>() : new Dictionary<string, object>(AdditionalAttributes);
        attrib.TryAdd("id", Id);

        AdditionalAttributes = new ReadOnlyDictionary<string, object>(attrib);
    }
}