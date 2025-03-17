using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Rizzy.Htmx;
using Rizzy.Utility;
using System.Collections.ObjectModel;

namespace Rizzy;

/// <summary>
/// An input component for editing Boolean values.
/// </summary>
public class RzInputCheckbox : InputCheckbox
{
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
    /// Called when the component is initialized.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        HttpContext?.GetOrAddFieldMapping(EditContext);
    }

    /// <summary>
    /// Called when the component has received parameters from its parent.
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (EditContext is null)
            throw new InvalidOperationException($"{nameof(RzInputCheckbox)} must be enclosed within an {nameof(EditForm)}.");

        if (string.IsNullOrEmpty(Id))
        {
            Id = IdGenerator.UniqueId(NameAttributeValue);
        }

        var fieldMapping = HttpContext?.GetOrAddFieldMapping(EditContext);

        if (fieldMapping != null && !fieldMapping.ContainsKey(FieldIdentifier))
        {
            fieldMapping[FieldIdentifier] = new RzFormFieldMap { FieldName = NameAttributeValue, Id = Id };
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
        var fieldMapping = HttpContext?.GetOrAddFieldMapping(EditContext);
        fieldMapping?.Remove(FieldIdentifier);

        base.Dispose(disposing);
    }
}
