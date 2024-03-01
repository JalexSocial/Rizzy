using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Rizzy.Components.Form.Helpers;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Rizzy.Components;

public class RzValidationMessage<TValue> : ValidationMessage<TValue>
{
    private IDictionary<string, object> _mergedAttributes = default!;
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

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (For is null)
            throw new InvalidOperationException($"{nameof(RzValidationMessage<TValue>)} requires a 'For' parameter.");

        var field = FieldIdentifier.Create(For);

        var messages = EditContext.GetValidationMessages(field);

        if (messages.Any())
            base.BuildRenderTree(builder);
        else
        {
            builder.OpenElement(10, "div");
            builder.AddAttribute(11, "class", "validation-message");
            builder.AddMultipleAttributes(12, AdditionalAttributes);
            builder.AddContent(13, string.Empty);
            builder.CloseElement();
        }
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
