using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Rizzy.Components.Form.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Rizzy.Components.Form;

/// <summary>
/// An input component used for selecting a value from a group of choices.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class RzInputRadio<TValue> : InputRadio<TValue>
{
	[Inject]
	public DataAnnotationsProcessor DataAnnotationsProcessor { get; set; } = default!;

	[CascadingParameter]
	private Dictionary<FieldIdentifier, string>? FieldMapping { get; set; }

	protected override void OnParametersSet()
	{
		base.OnParametersSet();

		if (FieldMapping is null)
			throw new InvalidOperationException($"{nameof(RzInputRadio<TValue>)} must be enclosed within an {nameof(RzEditForm)}.");

		// No validation
	}
}