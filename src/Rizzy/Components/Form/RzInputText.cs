using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Rizzy.Components.Form;
using Rizzy.Components.Form.Helpers;
using Rizzy.Htmx;
using Rizzy.Utility;

namespace Rizzy;

/// <summary>
/// A custom input text component that extends the Blazor InputText component.
/// </summary>
public class RzInputText : InputText
{
    /// <summary>
    /// Gets or sets the DataAnnotationsProcessor used to process data annotations.
    /// </summary>
    [Inject]
    public DataAnnotationsProcessor DataAnnotationsProcessor { get; set; } = default!;

    /// <summary>
    /// Gets or sets the HttpContext for the current request.
    /// </summary>
    [CascadingParameter]
    public HttpContext? HttpContext { get; set; }

    /// <summary>
    /// Gets or sets the ID of the input element.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Called when the component's parameters are set.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the component is not enclosed within an EditForm.</exception>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (EditContext is null)
            throw new InvalidOperationException($"{nameof(RzInputText)} must be enclosed within an {nameof(EditForm)}.");

        // If id doesn't exist then attempt to create one
        if (string.IsNullOrEmpty(Id) && AdditionalAttributes != null)
        {
            Id = IdGenerator.UniqueId(NameAttributeValue);
        }

        // Get the field mapping dictionary for the given EditContext.
        var fieldMapping = HttpContext?.GetOrAddFieldMapping(EditContext);

        // Add mapping for this field (use FieldIdentifier from the base class).
        if (fieldMapping != null && !fieldMapping.ContainsKey(FieldIdentifier))
        {
            fieldMapping[FieldIdentifier] = new RzFormFieldMap { FieldName = NameAttributeValue, Id = Id };
        }

        AdditionalAttributes = DataAnnotationsProcessor.MergeAttributes(nameof(RzInputText), ValueExpression, AdditionalAttributes, Id);
    }

    /// <summary>
    /// Disposes the component and removes the field mapping.
    /// </summary>
    /// <param name="disposing">A boolean value indicating whether the component is being disposed.</param>
    protected override void Dispose(bool disposing)
    {
        // When disposing, remove the field mapping.
        var fieldMapping = HttpContext?.GetOrAddFieldMapping(EditContext);
        fieldMapping?.Remove(FieldIdentifier);

        base.Dispose(disposing);
    }
}
