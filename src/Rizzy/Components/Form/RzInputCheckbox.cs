﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rizzy.Components.Form;
using Rizzy.Components.Form.Helpers;
using System.Collections.ObjectModel;

namespace Rizzy.Components;

/// <summary>
/// An input component for editing Boolean values.
/// </summary>
public class RzInputCheckbox : InputCheckbox
{
    [Inject]
    public DataAnnotationsProcessor DataAnnotationsProcessor { get; set; } = default!;

    [CascadingParameter]
    private RzEditForm? EditForm { get; set; }

    [Parameter]
    public string Id { get; set; } = string.Empty;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (EditForm is null)
            throw new InvalidOperationException($"{nameof(RzInputCheckbox)} must be enclosed within an {nameof(RzEditForm)}.");

        // No validation

        // If id doesn't exist then attempt to create one
        if (string.IsNullOrEmpty(Id))
        {
            Id = EditForm.CreateSanitizedId(NameAttributeValue);
        }

        EditForm.AddFieldMapping(FieldIdentifier, NameAttributeValue, Id);

        var attrib = AdditionalAttributes is null ? new Dictionary<string, object>() : new Dictionary<string, object>(AdditionalAttributes);
        attrib.TryAdd("id", Id);

        AdditionalAttributes = new ReadOnlyDictionary<string, object>(attrib);
    }

    protected override void Dispose(bool disposing)
    {
	    EditForm?.RemoveFieldMapping(FieldIdentifier);

	    base.Dispose(disposing);
    }

}