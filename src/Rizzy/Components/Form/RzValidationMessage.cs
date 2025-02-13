using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Http;
using Rizzy.Components.Form.Helpers;
using System.Collections.ObjectModel;
using System.Globalization;
using Rizzy.Htmx;

namespace Rizzy.Components;

public class RzValidationMessage<TValue> : ValidationMessage<TValue>
{
    private IDictionary<string, object> _mergedAttributes = default!;
    private string? _formattedValueExpression;
    private bool _shouldGenerateFieldNames;
    [CascadingParameter] EditContext EditContext { get; set; } = default!;

    [CascadingParameter]
    public HttpContext? HttpContext { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (EditContext is null)
            throw new InvalidOperationException($"{nameof(RzValidationMessage<TValue>)} must be enclosed within an {nameof(EditForm)}.");

        // Initialize or clear the merged attributes dictionary
        _mergedAttributes = AdditionalAttributes is null ? new Dictionary<string, object>() : new Dictionary<string, object>(AdditionalAttributes);
        _shouldGenerateFieldNames = EditContext.ShouldUseFieldIdentifiers;

        // Check if the "For" attribute is provided and extract the field name
        if (For is null)
            throw new InvalidOperationException($"{nameof(RzValidationMessage<TValue>)} requires a 'For' parameter.");

        var field = FieldIdentifier.Create(For);

        var fieldMapping = HttpContext?.GetOrAddFieldMapping(EditContext);

        string fieldName;
        if (fieldMapping != null && fieldMapping.TryGetValue(field, out var mapping))
        {
            fieldName = mapping.FieldName;
        }
        else
        {
            // Fallback
            fieldName = NameAttributeValue;
        }

        // Merge or add the new attributes
        _mergedAttributes["data-valmsg-for"] = fieldName;
        _mergedAttributes["data-valmsg-replace"] = "true";

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
        {
            foreach (var message in EditContext.GetValidationMessages(field))
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "validation-message field-validation-error");
                builder.AddMultipleAttributes(2, AdditionalAttributes);
                builder.AddContent(3, message);
                builder.CloseElement();
            }
        }
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
