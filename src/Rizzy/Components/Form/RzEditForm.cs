using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Rizzy.Components.Form.Models;
using Rizzy.Framework.Services;

namespace Rizzy.Components;

public class RzEditForm : ComponentBase
{
	public class FieldMap
	{
		public string Id { get; set; } = string.Empty;
		public string FieldName { get; set; } = string.Empty;
	}

    [Inject] public RzViewContext ViewContext { get; set; } = default!;

    [Parameter] public RzFormContext FormContext { get; set; } = default!;

    [Parameter] public RenderFragment<EditContext>? ChildContent { get; set; }

    // This dictionary is to capture unmatched values.
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    public Dictionary<FieldIdentifier, FieldMap> FieldMapping { get; internal set; } = new();

    /// <summary>
    /// Adds a mapping in FieldMapping to map a particular field identified by a FieldIdentifier to
    /// a FieldMap object that contains the fields form name and id. This is public to allow for third-party
    /// components to also map fields
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fieldName"></param>
    /// <param name="id"></param>
    public void AddFieldMapping(FieldIdentifier key, string fieldName, string id)
    {
	    if (FieldMapping.TryAdd(key, new FieldMap { FieldName = fieldName, Id = id}))
	        StateHasChanged();
    }

    protected override void OnParametersSet()
    {
        if (ViewContext is null)
            throw new InvalidOperationException($"{nameof(RzViewContext)} must be registered as a service");

        if (FormContext is null)
            throw new ArgumentException($"{nameof(RzFormContext)} is required");

        // If form id value set as an attribute then honor that
        if (AdditionalAttributes?.ContainsKey("id") == true)
            throw new InvalidOperationException("The form id value must be supplied only from the FormContext parameter");
        
        base.OnParametersSet();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<CascadingValue<RzEditForm>>(0);
        builder.AddAttribute(1, "Value", this);
        builder.AddAttribute(2, "IsFixed", true);
        builder.AddAttribute(3, "ChildContent", new RenderFragment((builder2) =>
        {
            builder2.OpenComponent<EditForm>(4);
            builder2.AddAttribute(5, "id", FormContext.Id);
            builder2.AddAttribute(6, "name", FormContext.FormName);

            builder2.AddAttribute(7, "EditContext", FormContext.EditContext);
            builder2.AddAttribute(8, "FormName", FormContext.FormName);
            builder2.AddAttribute(9, "ChildContent", ChildContent);

            // Add form name as a class or other attribute
            if (!string.IsNullOrWhiteSpace(FormContext.FormName))
            {
                builder2.AddAttribute(10, "FormName", FormContext.FormName);
            }

            // Use AddMultipleAttributes to add additional attributes
            if (AdditionalAttributes != null)
            {
                builder2.AddMultipleAttributes(11, AdditionalAttributes);
            }
            builder2.CloseComponent();
        }));
        builder.CloseComponent();
    }

    public string CreateSanitizedId(string fullname)
    {
	    if (string.IsNullOrEmpty(fullname))
	    {
		    return string.Empty;
	    }

	    // Remove leading and trailing spaces, replace spaces with hyphens, and remove invalid characters
	    string sanitized = Regex.Replace(fullname.Trim(), "\\s+", "-");
	    sanitized = Regex.Replace(sanitized, "[^a-zA-Z0-9\\-_:.]", "");

	    // Ensure the ID starts with a letter or an underscore (for broader compatibility and CSS friendliness)
	    if (!char.IsLetter(sanitized[0]) && sanitized[0] != '_')
	    {
		    sanitized = "rz" + sanitized;
	    }

	    return sanitized;
    }
}
