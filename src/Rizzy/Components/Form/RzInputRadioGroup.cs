﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Rizzy.Components.Form;
using Rizzy.Components.Form.Helpers;
using Rizzy.Utility;
using System.Collections.ObjectModel;
using Rizzy.Htmx;

namespace Rizzy.Components;

/// <summary>
/// Groups child InputRadio components.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class RzInputRadioGroup<TValue> : InputRadioGroup<TValue>
{
    [Inject]
    public DataAnnotationsProcessor DataAnnotationsProcessor { get; set; } = default!;

    [CascadingParameter]
    public HttpContext? HttpContext { get; set; }

    [Parameter]
    public string Id { get; set; } = string.Empty;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (EditContext is null)
            throw new InvalidOperationException($"{nameof(RzInputRadioGroup<TValue>)} must be enclosed within an {nameof(EditForm)}.");

        // No validation

        // If id doesn't exist then attempt to create one
        if (string.IsNullOrEmpty(Id))
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

        var attrib = AdditionalAttributes is null ? new Dictionary<string, object>() : new Dictionary<string, object>(AdditionalAttributes);
        attrib.TryAdd("id", Id);

        AdditionalAttributes = new ReadOnlyDictionary<string, object>(attrib);
    }

    protected override void Dispose(bool disposing)
    {
        // When disposing, remove the field mapping.
        var fieldMapping = HttpContext?.GetOrAddFieldMapping(EditContext);
        fieldMapping?.Remove(FieldIdentifier);

        base.Dispose(disposing);
    }

}