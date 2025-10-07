
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Rizzy.Htmx;
using Rizzy.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rizzy;

/// <summary>
/// An input component for editing date values.
/// </summary>
/// <typeparam name="TValue"></typeparam>
[RizzyParameterize]
public partial class RzInputDate<TValue> : InputDate<TValue>
{
    // Store the specific field mapping dictionary and identifier
    private IDictionary<FieldIdentifier, RzFormFieldMap>? _fieldMapping;
    private FieldIdentifier _fieldIdentifier;
    
    [Inject]
    public DataAnnotationsProcessor DataAnnotationsProcessor { get; set; } = default!;

    [CascadingParameter]
    public HttpContext? HttpContext { get; set; }

    [Parameter]
    public string Id { get; set; } = string.Empty;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        if (string.IsNullOrEmpty(Id))
        {
            // NameAttributeValue might be empty without an EditContext, so provide a fallback.
            Id = IdGenerator.UniqueId(NameAttributeValue ?? "rzdate");
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
                _fieldMapping[_fieldIdentifier] = new RzFormFieldMap { FieldName = NameAttributeValue, Id = Id };
            }

            AdditionalAttributes = DataAnnotationsProcessor.MergeAttributes(nameof(RzInputDate<TValue>), ValueExpression, AdditionalAttributes, Id);
        }
        else
        {
            var attrib = AdditionalAttributes is null ? new Dictionary<string, object>() : new Dictionary<string, object>(AdditionalAttributes);
            attrib.TryAdd("id", Id);
            AdditionalAttributes = new ReadOnlyDictionary<string, object>(attrib);
        }
    }

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