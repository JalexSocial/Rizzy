using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Rizzy.Framework.Mvc;

namespace Rizzy.Components.Form;

public class RzEditForm : ComponentBase
{
	[Inject]
	public RzViewContext ViewContext { get; set; } = default!;

	[Parameter]
	public EditContext? EditContext { get; set; }

	[Parameter]
	public RenderFragment<EditContext>? ChildContent { get; set; }

	[Parameter]
	public string? FormName { get; set; }

	// This dictionary is to capture unmatched values.
	[Parameter(CaptureUnmatchedValues = true)]
	public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

	private Dictionary<FieldIdentifier, string> _fieldMapping = new();

	protected override void OnParametersSet()
	{
		if (ViewContext is null)
			throw new InvalidOperationException($"{nameof(RzViewContext)} must be registered as a service");

		EditContext = ViewContext.EditContext;

		base.OnParametersSet();
	}

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenComponent<CascadingValue<Dictionary<FieldIdentifier, string>>>(0);
		builder.AddAttribute(1, "Value", _fieldMapping);
		builder.AddAttribute(2, "IsFixed", true);
		builder.AddAttribute(3, "ChildContent", new RenderFragment((builder2) =>
		{
			builder2.OpenComponent<EditForm>(4);
			builder2.AddAttribute(5, "EditContext", EditContext);

			builder2.AddAttribute(6, "ChildContent", ChildContent);

			// Add form name as a class or other attribute
			if (!string.IsNullOrWhiteSpace(FormName))
			{
				builder2.AddAttribute(7, "FormName", FormName); // Example, adjust as needed
			}

			// Use AddMultipleAttributes to add additional attributes
			if (AdditionalAttributes != null)
			{
				builder2.AddMultipleAttributes(8, AdditionalAttributes);
			}
			builder2.CloseComponent();
		}));
		builder.CloseComponent();
	}
}
