
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Rizzy.Htmx;
using Rizzy.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rizzy;

/// <summary>
/// A custom input text area component that extends the Blazor InputTextArea component.
/// </summary>
[RizzyParameterize] 
public partial class RzInputTextAreaBase : InputTextArea
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
    /// Gets or sets the ID of the text area.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Method invoked when the component has received parameters from its parent in the render tree.
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (string.IsNullOrEmpty(Id))
        {
            // NameAttributeValue might be empty without an EditContext, so provide a fallback.
            Id = IdGenerator.UniqueId(NameAttributeValue ?? "rztextarea");
        }

        if (EditContext is not null)
        {
            // Store the FieldIdentifier locally
            _fieldIdentifier = FieldIdentifier;
            
            // Get and store the fieldMapping dictionary instance
            _fieldMapping = HttpContext?.GetOrAddFieldMapping(EditContext);

            // Add mapping for this field (use FieldIdentifier from the base class).
            if (_fieldMapping != null && !_fieldMapping.ContainsKey(_fieldIdentifier))
            {
                _fieldMapping[_fieldIdentifier] = new RzFormFieldMap { FieldName = NameAttributeValue!, Id = Id };
            }

            AdditionalAttributes = DataAnnotationsProcessor?.MergeAttributes(nameof(RzInputTextAreaBase), ValueExpression, AdditionalAttributes, Id) ?? AdditionalAttributes;
        }
        else
        {
            var attrib = AdditionalAttributes is null ? new Dictionary<string, object>() : new Dictionary<string, object>(AdditionalAttributes);
            attrib.TryAdd("id", Id);
            AdditionalAttributes = new ReadOnlyDictionary<string, object>(attrib);
        }
    }

    /// <summary>
    /// Releases the unmanaged resources used by the component and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">A boolean value indicating whether the method has been called directly or indirectly by a user's code.</param>
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