using Microsoft.AspNetCore.Components.Forms;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Rizzy.Components.Form.Helpers;

/// <summary>
/// Processes Data Annotations on model properties and converts them to HTML5 data attributes.
/// Registration Scope: Singleton
/// </summary>
public class DataAnnotationsProcessor
{
    private IServiceProvider _provider;

    private readonly Dictionary<Type, Action<ValidationAttribute, IDictionary<string, object>, string>> _attributeHandlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataAnnotationsProcessor"/> class.
    /// </summary>
    public DataAnnotationsProcessor(IServiceProvider provider)
    {
        _provider = provider;

        // Default handlers for built-in types
        _attributeHandlers = new Dictionary<Type, Action<ValidationAttribute, IDictionary<string, object>, string>>
        {
            { typeof(RequiredAttribute), HandleRequiredAttribute },
            { typeof(StringLengthAttribute), HandleStringLengthAttribute },
            { typeof(RangeAttribute), HandleRangeAttribute },
            { typeof(RegularExpressionAttribute), HandleRegularExpressionAttribute },
            { typeof(CompareAttribute), HandleCompareAttribute },
            { typeof(EmailAddressAttribute), HandleEmailAddressAttribute },
            { typeof(PhoneAttribute), HandlePhoneAttribute },
            { typeof(UrlAttribute), HandleUrlAttribute },
            { typeof(MinLengthAttribute), HandleMinLengthAttribute },
            { typeof(MaxLengthAttribute), HandleMaxLengthAttribute },
        };
    }

    /// <summary>
    /// Adds a custom handler for a specific type of <see cref="ValidationAttribute"/>.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute to handle.</typeparam>
    /// <param name="handler">The handler that processes the attribute.</param>
    public void AddAttributeHandler<TAttribute>(Action<TAttribute, IDictionary<string, object>, string> handler)
        where TAttribute : ValidationAttribute
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        // Use a wrapper to match the expected delegate signature.
        _attributeHandlers[typeof(TAttribute)] = (attribute, attributes, propertyName) =>
            handler((TAttribute)attribute, attributes, propertyName);
    }

    public IReadOnlyDictionary<string, object>? MergeAttributes<TValue>(string controlName, Expression<Func<TValue>>? valueExpression, IReadOnlyDictionary<string, object>? additionalAttributes)
    {
        if (valueExpression == null)
        {
            throw new InvalidOperationException($"{controlName} requires a ValueExpression parameter.");
        }

        var attrib = additionalAttributes is null ?
                new Dictionary<string, object>() : new Dictionary<string, object>(additionalAttributes);
        var fieldIdentifier = FieldIdentifier.Create(valueExpression);
        ProcessAttributes(fieldIdentifier, attrib);

        if (attrib.Keys.Count == 0)
            return null;

        return new ReadOnlyDictionary<string, object>(attrib);
    }

    /// <summary>
    /// Processes the attributes of a specified field and converts them to HTML5 data attributes.
    /// </summary>
    /// <param name="fieldIdentifier">The identifier of the field to process.</param>
    /// <param name="attributes">The collection where HTML5 data attributes should be added.</param>
    private void ProcessAttributes(FieldIdentifier fieldIdentifier, IDictionary<string, object> attributes)
    {
        var propertyInfo = fieldIdentifier.Model.GetType().GetProperty(fieldIdentifier.FieldName);
        if (propertyInfo == null)
        {
            throw new InvalidOperationException($"The property {fieldIdentifier.FieldName} was not found on the model of type {fieldIdentifier.Model.GetType().FullName}.");
        }

        var validationAttributes = propertyInfo.GetCustomAttributes(true).OfType<ValidationAttribute>().ToList();

        // Initialize client-side validation if any validation attributes are present.
        if (validationAttributes.Any())
        {
            attributes.TryAdd("data-val", "true");
        }

        foreach (var attribute in validationAttributes)
        {
            if (_attributeHandlers.TryGetValue(attribute.GetType(), out var handler))
            {
                handler(attribute, attributes, propertyInfo.Name);
            }
        }
    }

    private void HandleRequiredAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string propertyName)
    {
        var requiredAttribute = (RequiredAttribute)attribute;
        attributes["data-val-required"] = requiredAttribute.ErrorMessage ?? $"{propertyName} is required.";
    }

    private void HandleStringLengthAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string propertyName)
    {
        var stringLengthAttribute = (StringLengthAttribute)attribute;
        attributes["data-val-length"] = stringLengthAttribute.ErrorMessage ?? $"The field {propertyName} must be a string with a maximum length of {stringLengthAttribute.MaximumLength}.";
        attributes["data-val-length-max"] = stringLengthAttribute.MaximumLength;
        if (stringLengthAttribute.MinimumLength > 0)
        {
            attributes["data-val-length-min"] = stringLengthAttribute.MinimumLength;
        }
    }

    private void HandleRangeAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string propertyName)
    {
        var rangeAttribute = (RangeAttribute)attribute;
        attributes["data-val-range"] = rangeAttribute.ErrorMessage ?? $"The field {propertyName} must be between {rangeAttribute.Minimum} and {rangeAttribute.Maximum}.";
        attributes["data-val-range-min"] = rangeAttribute.Minimum;
        attributes["data-val-range-max"] = rangeAttribute.Maximum;
    }

    private void HandleRegularExpressionAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string propertyName)
    {
        var regexAttribute = (RegularExpressionAttribute)attribute;
        attributes["data-val-regex"] = regexAttribute.ErrorMessage ?? $"The field {propertyName} must match the regular expression '{regexAttribute.Pattern}'.";
        attributes["data-val-regex-pattern"] = regexAttribute.Pattern;
    }

    private void HandleCompareAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string propertyName)
    {
        var compareAttribute = (CompareAttribute)attribute;
        attributes["data-val-equalto"] = compareAttribute.ErrorMessage ?? $"The field {propertyName} must be equal to {compareAttribute.OtherProperty}.";
        attributes["data-val-equalto-other"] = $"*.{compareAttribute.OtherProperty}";
    }

    private void HandleEmailAddressAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string propertyName)
    {
        attributes["data-val-email"] = attribute.ErrorMessage ?? $"The field {propertyName} must be a valid email address.";
    }

    private void HandlePhoneAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string propertyName)
    {
        attributes["data-val-phone"] = attribute.ErrorMessage ?? $"The field {propertyName} must be a valid phone number.";
    }

    private void HandleUrlAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string propertyName)
    {
        attributes["data-val-url"] = attribute.ErrorMessage ?? $"The field {propertyName} must be a valid URL.";
    }

    private void HandleMinLengthAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string propertyName)
    {
        var minLengthAttribute = (MinLengthAttribute)attribute;
        attributes["data-val-minlength"] = minLengthAttribute.ErrorMessage ?? $"The field {propertyName} must be at least {minLengthAttribute.Length} characters.";
        attributes["data-val-minlength-min"] = minLengthAttribute.Length;
    }

    private void HandleMaxLengthAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string propertyName)
    {
        var maxLengthAttribute = (MaxLengthAttribute)attribute;
        attributes["data-val-maxlength"] = maxLengthAttribute.ErrorMessage ?? $"The field {propertyName} cannot exceed {maxLengthAttribute.Length} characters.";
        attributes["data-val-maxlength-max"] = maxLengthAttribute.Length;
    }
}

