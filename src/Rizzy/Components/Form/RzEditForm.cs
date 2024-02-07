using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Rizzy.Components.Form.Models;
using Rizzy.Framework.Mvc;

namespace Rizzy.Components.Form;

public class RzEditForm : ComponentBase
{
    [Inject] public RzViewContext ViewContext { get; set; } = default!;

    [Parameter] public RzFormContext FormContext { get; set; } = default!;

    [Parameter] public RenderFragment<EditContext>? ChildContent { get; set; }

    // This dictionary is to capture unmatched values.
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private Dictionary<FieldIdentifier, string> _fieldMapping = new();

    protected override void OnParametersSet()
    {
        if (ViewContext is null)
            throw new InvalidOperationException($"{nameof(RzViewContext)} must be registered as a service");

        if (FormContext is null)
            throw new ArgumentException($"{nameof(RzFormContext)} is required");

        // If form id value set as an attribute then honor that
        if (AdditionalAttributes?.ContainsKey("id") == true)
            throw new InvalidOperationException("The form id value must be supplied only from the FormContext parameter");

        base.OnParametersSet();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<CascadingValue<Dictionary<FieldIdentifier, string>>>(0);
        builder.AddAttribute(1, "Value", _fieldMapping);
        builder.AddAttribute(2, "IsFixed", true);
        builder.AddAttribute(3, "ChildContent", new RenderFragment((builder2) =>
        {
            builder2.OpenComponent<EditForm>(4);
            builder2.AddAttribute(5, "id", FormContext.Id);
            builder2.AddAttribute(6, "name", FormContext.FormName);

            builder2.AddAttribute(7, "EditContext", FormContext.EditContext);
            builder2.AddAttribute(8, "FormName", FormContext.FormName);
            builder2.AddAttribute(9, "ChildContent", ChildContent);

            // Add form name as a class or other attribute
            if (!string.IsNullOrWhiteSpace(FormContext.FormName))
            {
                builder2.AddAttribute(10, "FormName", FormContext.FormName);
            }

            // Use AddMultipleAttributes to add additional attributes
            if (AdditionalAttributes != null)
            {
                builder2.AddMultipleAttributes(11, AdditionalAttributes);
            }
            builder2.CloseComponent();
        }));
        builder.CloseComponent();
    }
}
