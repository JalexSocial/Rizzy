using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rizzy.Components.Form.Helpers;

namespace Rizzy.Components;

/// <summary>
/// An input component for editing date values.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class RzInputDate<TValue> : InputDate<TValue>
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
            throw new InvalidOperationException($"{nameof(RzInputDate<TValue>)} must be enclosed within an {nameof(RzEditForm)}.");

        FieldMapping.TryAdd(FieldIdentifier, NameAttributeValue);

        // Check if an id is already present for this input
        if (AdditionalAttributes != null && AdditionalAttributes.TryGetValue("id", out object? value))
        {
	        if (value is string idValue)
	        {
		        Id = idValue;
	        }
        }

        // If id doesn't exist then attempt to create one
        if (string.IsNullOrEmpty(Id))
        {
	        Id = IdProvider.CreateSanitizedId(NameAttributeValue);
        }

        AdditionalAttributes = DataAnnotationsProcessor.MergeAttributes(nameof(RzInputDate<TValue>), ValueExpression, AdditionalAttributes, Id);
    }
}