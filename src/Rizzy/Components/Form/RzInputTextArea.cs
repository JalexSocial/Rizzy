using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Rizzy.Htmx;
using Rizzy.Utility;
using System.Collections.Generic;

namespace Rizzy;

/// <summary>
/// A custom input text area component that extends the Blazor InputTextArea component.
/// </summary>
[RizzyParameterize] 
public partial class RzInputTextArea : InputTextArea
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

    // Ensures expensive initialization logic is executed just once.
    private bool _initialized;
    
    /// <summary>
    /// Method invoked when the component has received parameters from its parent in the render tree.
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (EditContext is null)
            throw new InvalidOperationException($"{nameof(RzInputTextArea)} must be enclosed within an {nameof(EditForm)}.");

        // Always store the FieldIdentifier locally, even if already initialized
        _fieldIdentifier = FieldIdentifier;
        
        // Always update the field mapping reference
        _fieldMapping = HttpContext?.GetOrAddFieldMapping(EditContext);
        
        // Skip the rest of initialization if already done
        if (_initialized) return;
        
        // If id doesn't exist then attempt to create one
        if (string.IsNullOrEmpty(Id))
        {
            Id = IdGenerator.UniqueId(NameAttributeValue);
        }

        // Add mapping for this field (use FieldIdentifier from the base class).
        if (_fieldMapping != null && !_fieldMapping.ContainsKey(_fieldIdentifier))
        {
            _fieldMapping[_fieldIdentifier] = new RzFormFieldMap { FieldName = NameAttributeValue, Id = Id };
        }

        AdditionalAttributes = DataAnnotationsProcessor.MergeAttributes(nameof(RzInputTextArea), ValueExpression, AdditionalAttributes, Id);

        _initialized = true;
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
