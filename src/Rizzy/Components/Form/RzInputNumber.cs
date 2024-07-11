using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rizzy.Components.Form;
using Rizzy.Components.Form.Helpers;

namespace Rizzy.Components;

/// <summary>
/// An input component for editing numeric values. Supported numeric types are
/// Int32, Int64, Int16, Single, Double, Decimal.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class RzInputNumber<TValue> : InputNumber<TValue>
{
    [Inject]
    public DataAnnotationsProcessor DataAnnotationsProcessor { get; set; } = default!;

    [CascadingParameter]
    private RzEditForm? EditForm { get; set; }

    [Parameter]
    public string Id { get; set; } = string.Empty;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (EditForm is null)
            throw new InvalidOperationException($"{nameof(RzInputNumber<TValue>)} must be enclosed within an {nameof(RzEditForm)}.");

        // If id doesn't exist then attempt to create one
        if (string.IsNullOrEmpty(Id))
        {
            Id = EditForm.CreateSanitizedId(NameAttributeValue);
        }

        EditForm.AddFieldMapping(FieldIdentifier, NameAttributeValue, Id);

        AdditionalAttributes = DataAnnotationsProcessor.MergeAttributes(nameof(RzInputNumber<TValue>), ValueExpression, AdditionalAttributes, Id);
    }

    protected override void Dispose(bool disposing)
    {
	    EditForm?.RemoveFieldMapping(FieldIdentifier);

	    base.Dispose(disposing);
    }

}