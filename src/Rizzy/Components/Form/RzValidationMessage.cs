using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rizzy.Components.Form.Helpers;
using Rizzy.Framework.Mvc;

namespace Rizzy.Components.Form;

public class RzValidationMessage<TValue> : ValidationMessage<TValue>
{
	private IDictionary<string, object> _mergedAttributes;
	private string? _formattedValueExpression;
	private bool _shouldGenerateFieldNames;
	[CascadingParameter] EditContext EditContext { get; set; } = default!;

	[CascadingParameter]
	private Dictionary<FieldIdentifier, string>? FieldMapping { get; set; }

	protected override void OnParametersSet()
	{
		base.OnParametersSet();

		if (FieldMapping is null)
			throw new InvalidOperationException($"{nameof(RzValidationMessage<TValue>)} must be enclosed within an {nameof(RzEditForm)}.");

		// Initialize or clear the merged attributes dictionary
		_mergedAttributes = new Dictionary<string, object>();
		_shouldGenerateFieldNames = EditContext.ShouldUseFieldIdentifiers;

		// Check if the "For" attribute is provided and extract the field name
		if (For is null)
			throw new InvalidOperationException($"{nameof(RzValidationMessage<TValue>)} requires a 'For' parameter.");

		var field = FieldIdentifier.Create(For);
		var fieldName = FieldMapping!.ContainsKey(field) ? FieldMapping[field] : NameAttributeValue;
		//var otherFieldName = NameAttributeValue;

		// Merge or add the new attributes
		_mergedAttributes["data-valmsg-for"] = fieldName;
		_mergedAttributes["data-valmsg-replace"] = "true";

		// Merge with existing AdditionalAttributes if any
		if (AdditionalAttributes != null)
		{
			foreach (var attribute in AdditionalAttributes)
			{
				_mergedAttributes[attribute.Key] = attribute.Value;
			}
		}

		AdditionalAttributes = new ReadOnlyDictionary<string, object>(_mergedAttributes);
	}

	/// <summary>
	/// This is fallback code pulled directly from the Asp.net source
	/// </summary>
	private string NameAttributeValue
	{
		get
		{
			if (AdditionalAttributes?.TryGetValue("name", out var nameAttributeValue) ?? false)
			{
				return Convert.ToString(nameAttributeValue, CultureInfo.InvariantCulture) ?? string.Empty;
			}

			if (_shouldGenerateFieldNames)
			{
				if (_formattedValueExpression is null && For is not null)
				{
					_formattedValueExpression = //FieldPrefix != null ? FieldPrefix.GetFieldName(For) :
						ExpressionFormatter.FormatLambda(For);
				}

				return _formattedValueExpression ?? string.Empty;
			}

			return string.Empty;
		}
	}
}
