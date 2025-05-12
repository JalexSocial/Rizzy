using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Rizzy.Htmx;
using Rizzy.Utility;
using System.Collections.Generic;

namespace Rizzy;

/// <summary>
/// An input component for editing numeric values. Supported numeric types are
/// Int32, Int64, Int16, Single, Double, Decimal.
/// </summary>
/// <typeparam name="TValue">The type of the value being edited.</typeparam>
[RizzyParameterize] 
public partial class RzInputNumber<TValue> : InputNumber<TValue>
{
    // Store the specific field mapping dictionary and identifier
    private IDictionary<FieldIdentifier, RzFormFieldMap>? _fieldMapping;
    private FieldIdentifier _fieldIdentifier;
    
    /// <summary>
    /// Gets or sets the <see cref="DataAnnotationsProcessor"/> used to process data annotations.
    /// </summary>
    [Inject]
    public DataAnnotationsProcessor DataAnnotationsProcessor { get; set; } = default!;

    /// <summary>
    /// Gets or sets the <see cref="HttpContext"/> for the current request.
    /// </summary>
    [CascadingParameter]
    public HttpContext? HttpContext { get; set; }

    /// <summary>
    /// Gets or sets the ID of the input element.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Method invoked when the component has received parameters from its parent in the render tree,
    /// and the incoming values have been assigned to properties.
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (EditContext is null)
            throw new InvalidOperationException($"{nameof(RzInputNumber<TValue>)} must be enclosed within an {nameof(EditForm)}.");

        // Store the FieldIdentifier locally
        _fieldIdentifier = FieldIdentifier;
        
        // If id doesn't exist then attempt to create one
        if (string.IsNullOrEmpty(Id))
        {
            Id = IdGenerator.UniqueId(NameAttributeValue);
        }

        // Get and store the fieldMapping dictionary instance
        _fieldMapping = HttpContext?.GetOrAddFieldMapping(EditContext);

        // Add mapping for this field (use FieldIdentifier from the base class).
        if (_fieldMapping != null && !_fieldMapping.ContainsKey(_fieldIdentifier))
        {
            _fieldMapping[_fieldIdentifier] = new RzFormFieldMap { FieldName = NameAttributeValue, Id = Id };
        }

        AdditionalAttributes = DataAnnotationsProcessor.MergeAttributes(nameof(RzInputNumber<TValue>), ValueExpression, AdditionalAttributes, Id);
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
