using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace Rizzy;

/// <summary>
/// Processes Data Annotations on model properties and converts them to HTML5 data attributes.
/// Registration Scope: Singleton
/// </summary>
public class DataAnnotationsProcessor
{
    private readonly IServiceProvider _provider;
    private readonly IValidationAttributeAdapterProvider _adapterProvider;
    private readonly IStringLocalizerFactory? _localizerFactory;
    private readonly IModelMetadataProvider _metadataProvider;

    // Handlers take a ValidationAttribute, the attributes dictionary, and a localized error message.
    private readonly Dictionary<Type, Action<ValidationAttribute, IDictionary<string, object>, string>> _attributeHandlers;
    private static readonly ConcurrentDictionary<Type, PropertyCacheEntry> _propertyCache = new();

    private class PropertyCacheEntry
    {
        public PropertyInfo PropertyInfo { get; }
        public ValidationAttribute[] ValidationAttributes { get; }

        public PropertyCacheEntry(PropertyInfo propInfo)
        {
            PropertyInfo = propInfo;
            ValidationAttributes = propInfo.GetCustomAttributes<ValidationAttribute>(true).ToArray();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataAnnotationsProcessor"/> class.
    /// </summary>
    /// <param name="provider">The application's service provider.</param>
    public DataAnnotationsProcessor(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _adapterProvider = provider.GetService<IValidationAttributeAdapterProvider>() ??
                           new ValidationAttributeAdapterProvider();
        // Attempt to resolve the localizer factory; if not present, _localizerFactory will be null.
        _localizerFactory = provider.GetService<IStringLocalizerFactory>();
        _metadataProvider = provider.GetService<IModelMetadataProvider>() ??
                            new EmptyModelMetadataProvider();

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

        _attributeHandlers[typeof(TAttribute)] = (attribute, attributes, message) =>
            handler((TAttribute)attribute, attributes, message);
    }

    /// <summary>
    /// Merges the data annotations from a model property into the provided attributes collection.
    /// </summary>
    /// <typeparam name="TValue">The type of the bound value.</typeparam>
    /// <param name="controlName">The name of the control being rendered.</param>
    /// <param name="valueExpression">The expression identifying the model property.</param>
    /// <param name="additionalAttributes">Any additional attributes.</param>
    /// <param name="id">Optional element id.</param>
    /// <returns>A read-only dictionary of HTML attributes (or null if none).</returns>
    public IReadOnlyDictionary<string, object>? MergeAttributes<TValue>(
        string controlName,
        Expression<Func<TValue>>? valueExpression,
        IReadOnlyDictionary<string, object>? additionalAttributes,
        string? id = null)
    {
        if (valueExpression == null)
        {
            throw new InvalidOperationException($"{controlName} requires a ValueExpression parameter.");
        }

        var attrib = additionalAttributes is null
            ? new Dictionary<string, object>()
            : new Dictionary<string, object>(additionalAttributes);

        if (id != null)
        {
            attrib.TryAdd("id", id);
        }

        var fieldIdentifier = FieldIdentifier.Create(valueExpression);
        ProcessAttributes(fieldIdentifier, attrib);

        return attrib.Count == 0 ? null : new ReadOnlyDictionary<string, object>(attrib);
    }

    /// <summary>
    /// Processes the attributes of a specified field and converts them to HTML5 data attributes.
    /// </summary>
    /// <param name="fieldIdentifier">The identifier of the field to process.</param>
    /// <param name="attributes">The dictionary where HTML5 data attributes should be added.</param>
    private void ProcessAttributes(FieldIdentifier fieldIdentifier, IDictionary<string, object> attributes)
    {
        var propertyEntry = GetCachedProperty(fieldIdentifier);

        var modelType = fieldIdentifier.Model.GetType();
        var metadata = _metadataProvider.GetMetadataForProperty(modelType, fieldIdentifier.FieldName);

        // If a localizer factory is available, create a localizer; otherwise, localizer remains null.
        IStringLocalizer? localizer = _localizerFactory?.Create(modelType);

        // Construct a minimal ActionContext.
        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor());

        // Correctly construct ModelValidationContextBase.
        var validationContext = new DefaultModelValidationContext(actionContext, metadata, _metadataProvider);

        if (propertyEntry.ValidationAttributes.Any())
        {
            attributes.TryAdd("data-val", "true");
        }

        foreach (var attribute in propertyEntry.ValidationAttributes)
        {
            // Attempt to obtain a localized error message; if unavailable, fall back to FormatErrorMessage.
            string localizedMessage = GetLocalizedErrorMessage(attribute, metadata, validationContext, localizer)
                ?? attribute.FormatErrorMessage(metadata.GetDisplayName());

            if (_attributeHandlers.TryGetValue(attribute.GetType(), out var handler))
            {
                handler(attribute, attributes, localizedMessage);
            }
        }
    }

    private PropertyCacheEntry GetCachedProperty(FieldIdentifier fieldIdentifier)
    {
        return _propertyCache.GetOrAdd(fieldIdentifier.Model.GetType(), type =>
        {
            var propInfo = type.GetProperty(fieldIdentifier.FieldName);
            if (propInfo == null)
            {
                throw new InvalidOperationException(
                    $"The property {fieldIdentifier.FieldName} was not found on the model of type {fieldIdentifier.Model.GetType().FullName}.");
            }
            return new PropertyCacheEntry(propInfo);
        });
    }

    private string? GetLocalizedErrorMessage(
        ValidationAttribute attribute,
        ModelMetadata metadata,
        ModelValidationContextBase validationContext,
        IStringLocalizer? localizer)
    {
        var adapter = _adapterProvider.GetAttributeAdapter(attribute, localizer);
        return adapter?.GetErrorMessage(validationContext);
    }

    // Default handler implementations that use the provided localized message.

    private static void HandleRequiredAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string message)
    {
        attributes["data-val-required"] = message;
    }

    private static void HandleStringLengthAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string message)
    {
        var stringLengthAttribute = (StringLengthAttribute)attribute;
        attributes["data-val-length"] = message;
        attributes["data-val-length-max"] = stringLengthAttribute.MaximumLength;
        if (stringLengthAttribute.MinimumLength > 0)
        {
            attributes["data-val-length-min"] = stringLengthAttribute.MinimumLength;
        }
    }

    private static void HandleRangeAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string message)
    {
        var rangeAttribute = (RangeAttribute)attribute;
        attributes["data-val-range"] = message;
        attributes["data-val-range-min"] = rangeAttribute.Minimum;
        attributes["data-val-range-max"] = rangeAttribute.Maximum;
    }

    private static void HandleRegularExpressionAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string message)
    {
        var regexAttribute = (RegularExpressionAttribute)attribute;
        attributes["data-val-regex"] = message;
        attributes["data-val-regex-pattern"] = regexAttribute.Pattern;
    }

    private static void HandleCompareAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string message)
    {
        var compareAttribute = (CompareAttribute)attribute;
        attributes["data-val-equalto"] = message;
        attributes["data-val-equalto-other"] = $"*.{compareAttribute.OtherProperty}";
    }

    private static void HandleEmailAddressAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string message)
    {
        attributes["data-val-email"] = message;
    }

    private static void HandlePhoneAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string message)
    {
        attributes["data-val-phone"] = message;
    }

    private static void HandleUrlAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string message)
    {
        attributes["data-val-url"] = message;
    }

    private static void HandleMinLengthAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string message)
    {
        var minLengthAttribute = (MinLengthAttribute)attribute;
        attributes["data-val-minlength"] = message;
        attributes["data-val-minlength-min"] = minLengthAttribute.Length;
    }

    private static void HandleMaxLengthAttribute(ValidationAttribute attribute, IDictionary<string, object> attributes, string message)
    {
        var maxLengthAttribute = (MaxLengthAttribute)attribute;
        attributes["data-val-maxlength"] = message;
        attributes["data-val-maxlength-max"] = maxLengthAttribute.Length;
    }

    /// <summary>
    /// A minimal implementation of ModelValidationContextBase using the correct constructor.
    /// </summary>
    private class DefaultModelValidationContext(
        ActionContext actionContext,
        ModelMetadata modelMetadata,
        IModelMetadataProvider metadataProvider)
        : ModelValidationContextBase(actionContext, modelMetadata, metadataProvider);
}