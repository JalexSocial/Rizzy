using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rizzy.Components.Form;
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
    private RzEditForm? EditForm { get; set; }

    [Parameter]
    public string Id { get; set; } = string.Empty;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (EditForm is null)
            throw new InvalidOperationException($"{nameof(RzInputDate<TValue>)} must be enclosed within an {nameof(RzEditForm)}.");

        // If id doesn't exist then attempt to create one
        if (string.IsNullOrEmpty(Id))
        {
            Id = EditForm.CreateSanitizedId(NameAttributeValue);
        }

        EditForm.AddFieldMapping(FieldIdentifier, NameAttributeValue, Id);

        AdditionalAttributes = DataAnnotationsProcessor.MergeAttributes(nameof(RzInputDate<TValue>), ValueExpression, AdditionalAttributes, Id);
    }
}