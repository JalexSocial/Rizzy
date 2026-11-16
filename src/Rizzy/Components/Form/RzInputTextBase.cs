
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Rizzy.Htmx;
using Rizzy.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rizzy;

/// <summary>
/// A custom input text component that extends the Blazor InputText component.
/// </summary>
[RizzyParameterize] 
public partial class RzInputTextBase : InputText
{
    // Store the specific field mapping dictionary and identifier
    private IDictionary<FieldIdentifier, RzFormFieldMap>? _fieldMapping;
    private FieldIdentifier _fieldIdentifier;
    
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
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (EditContext is not null)
        {
            // Registers the field for labels and validation messages, adopting an id a label reserved.
            _fieldIdentifier = FieldIdentifier;
            Id = FormFieldRegistration.Register(HttpContext, EditContext, _fieldIdentifier, NameAttributeValue, Id, "rzinput", out _fieldMapping);

            AdditionalAttributes = DataAnnotationsProcessor?.MergeAttributes(nameof(RzInputTextBase), ValueExpression, AdditionalAttributes, Id) ?? AdditionalAttributes;
        }
        else
        {
            if (string.IsNullOrEmpty(Id))
                Id = IdGenerator.UniqueId(NameAttributeValue ?? "rzinput");

            var attrib = AdditionalAttributes is null ? new Dictionary<string, object>() : new Dictionary<string, object>(AdditionalAttributes);
            attrib.TryAdd("id", Id);
            AdditionalAttributes = new ReadOnlyDictionary<string, object>(attrib);
        }
    }

    /// <summary>
    /// Disposes the component and removes the field mapping.
    /// </summary>
    /// <param name="disposing">A boolean value indicating whether the component is being disposed.</param>
    protected override void Dispose(bool disposing)
    {
        // Use the locally stored fieldMapping and fieldIdentifier for removal
        // This avoids accessing the potentially null EditContext here.
        if (disposing && _fieldMapping != null)
        {
            _fieldMapping.Remove(_fieldIdentifier);
            _fieldMapping = null; // Optional: Clear the reference
        }

        base.Dispose(disposing);
    }
}