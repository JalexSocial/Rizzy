using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rizzy.Components.Form.Helpers;

namespace Rizzy.Components;

public class RzInputText : InputText
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
            throw new InvalidOperationException($"{nameof(RzInputText)} must be enclosed within an {nameof(RzEditForm)}.");

        FieldMapping.TryAdd(FieldIdentifier, NameAttributeValue);

        // If id doesn't exist then attempt to create one
        if (string.IsNullOrEmpty(Id))
        {
	        Id = IdProvider.CreateSanitizedId(NameAttributeValue);
        }

        AdditionalAttributes = DataAnnotationsProcessor.MergeAttributes(nameof(RzInputText), ValueExpression, AdditionalAttributes, Id);
    }
}