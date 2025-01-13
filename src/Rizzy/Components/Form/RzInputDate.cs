using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rizzy.Components.Form;
using Rizzy.Components.Form.Helpers;
using Rizzy.Utility;

namespace Rizzy.Components;

/// <summary>
/// An input component for editing date values.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class RzInputDate<TValue> : InputDate<TValue>
{
    [Inject]
    public DataAnnotationsProcessor DataAnnotationsProcessor { get; set; } = default!;

    [Inject]
    public RzViewContext ViewContext { get; set; } = default!;

    [Parameter]
    public string Id { get; set; } = string.Empty;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (EditContext is null)
            throw new InvalidOperationException($"{nameof(RzInputDate<TValue>)} must be enclosed within an {nameof(EditForm)}.");

        // If id doesn't exist then attempt to create one
        if (string.IsNullOrEmpty(Id))
        {
            Id = IdGenerator.UniqueId(NameAttributeValue);
        }

        // Get the field mapping dictionary for the given EditContext.
        var fieldMapping = ViewContext.GetOrAddFieldMapping(EditContext);

        // Add mapping for this field (use FieldIdentifier from the base class).
        if (!fieldMapping.ContainsKey(FieldIdentifier))
        {
            fieldMapping[FieldIdentifier] = new RzFormFieldMap { FieldName = NameAttributeValue, Id = Id };
        }

        AdditionalAttributes = DataAnnotationsProcessor.MergeAttributes(nameof(RzInputDate<TValue>), ValueExpression, AdditionalAttributes, Id);
    }

    protected override void Dispose(bool disposing)
    {
        // When disposing, remove the field mapping.
        var fieldMapping = ViewContext.GetOrAddFieldMapping(EditContext);
        fieldMapping.Remove(FieldIdentifier);

        base.Dispose(disposing);
    }

}