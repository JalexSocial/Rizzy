using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Rizzy.Htmx;
using Rizzy.Utility;
using System.Collections.ObjectModel;
using System.Collections.Generic; 

namespace Rizzy;

/// <summary>
/// An input component for editing Boolean values.
/// </summary>
[RizzyParameterize] 
public partial class RzInputCheckbox : InputCheckbox
{
    // Store the specific field mapping dictionary and identifier
    private IDictionary<FieldIdentifier, RzFormFieldMap>? _fieldMapping;
    private FieldIdentifier _fieldIdentifier;

    /// <summary>
    /// Gets or sets the data annotations processor service.
    /// </summary>
    [Inject]
    public DataAnnotationsProcessor DataAnnotationsProcessor { get; set; } = default!;

    /// <summary>
    /// Gets or sets the HTTP context that can be used to retrieve field mappings.
    /// </summary>
    [CascadingParameter]
    public HttpContext? HttpContext { get; set; }

    /// <summary>
    /// Gets or sets the id for the checkbox element.
    /// An auto-generated id will be assigned if this is not set.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Called when the component has received parameters from its parent.
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (EditContext is null)
            throw new InvalidOperationException($"{nameof(RzInputCheckbox)} must be enclosed within an {nameof(EditForm)}.");

        // Store the FieldIdentifier locally
        _fieldIdentifier = FieldIdentifier;

        if (string.IsNullOrEmpty(Id))
        {
            Id = IdGenerator.UniqueId(NameAttributeValue);
        }

        // Get and store the fieldMapping dictionary instance
        _fieldMapping = HttpContext?.GetOrAddFieldMapping(EditContext);

        // Use the local variable for the add operation
        if (_fieldMapping != null && !_fieldMapping.ContainsKey(_fieldIdentifier))
        {
            _fieldMapping[_fieldIdentifier] = new RzFormFieldMap { FieldName = NameAttributeValue, Id = Id };
        }

        var attrib = AdditionalAttributes is null ? new Dictionary<string, object>() : new Dictionary<string, object>(AdditionalAttributes);
        attrib.TryAdd("id", Id);
        AdditionalAttributes = new ReadOnlyDictionary<string, object>(attrib);
    }

    /// <summary>
    /// Disposes resources used by this component.
    /// </summary>
    /// <param name="disposing">Whether the component is disposing.</param>
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