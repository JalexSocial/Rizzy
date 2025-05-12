using Rizzy.State.Attributes; // For RizzyStateAttribute
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rizzy.State.Serialization;

/// <summary>
/// Default implementation of <see cref="IRizzyViewModelSerializer"/>.
/// Serializes and deserializes properties marked with <see cref="RizzyStateAttribute"/>.
/// </summary>
public class RizzyViewModelSerializer : IRizzyViewModelSerializer
{
    private readonly JsonSerializerOptions _jsonOptions;

    // Cache for property information to improve performance
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RizzyViewModelSerializer"/> class.
    /// </summary>
    /// <param name="jsonOptions">Optional JsonSerializerOptions. If null, default options with CamelCase naming and null value ignoring will be used.</param>
    public RizzyViewModelSerializer(JsonSerializerOptions? jsonOptions = null)
    {
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            // Consider adding Rizzy-specific converters here if needed in the future
        };
    }

    private static PropertyInfo[] GetPersistentProperties(Type viewModelType)
    {
        return _propertyCache.GetOrAdd(viewModelType, type =>
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.IsDefined(typeof(RizzyStateAttribute), inherit: true) && p.CanRead && p.CanWrite)
                .ToArray());
    }

    /// <inheritdoc />
    public IDictionary<string, object?> ExtractPersistentState<TViewModel>(TViewModel viewModel) where TViewModel : class
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        var stateToSerialize = new Dictionary<string, object?>();
        var properties = GetPersistentProperties(typeof(TViewModel));

        foreach (var prop in properties)
        {
            // Use the configured JsonNamingPolicy to get the name for serialization (e.g., camelCase)
            string jsonPropertyName = _jsonOptions.PropertyNamingPolicy?.ConvertName(prop.Name) ?? prop.Name;
            stateToSerialize[jsonPropertyName] = prop.GetValue(viewModel);
        }
        return stateToSerialize;
    }

    /// <inheritdoc />
    public void PopulateViewModel<TViewModel>(TViewModel viewModel, IDictionary<string, JsonElement> persistentState) where TViewModel : class
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(persistentState);

        var propertiesToSet = GetPersistentProperties(typeof(TViewModel));

        foreach (var prop in propertiesToSet)
        {
            // Keys in persistentState dictionary are expected to be as serialized (e.g., camelCase)
            string jsonPropertyName = _jsonOptions.PropertyNamingPolicy?.ConvertName(prop.Name) ?? prop.Name;

            if (persistentState.TryGetValue(jsonPropertyName, out JsonElement jsonValue))
            {
                if (jsonValue.ValueKind != JsonValueKind.Null && jsonValue.ValueKind != JsonValueKind.Undefined)
                {
                    try
                    {
                        object? propValue = jsonValue.Deserialize(prop.PropertyType, _jsonOptions);
                        prop.SetValue(viewModel, propValue);
                    }
                    catch (JsonException ex)
                    {
                        // Consider logging this error. For now, it might skip setting the property if conversion fails.
                        // This could happen if the token structure changes or is incompatible.
                        Console.Error.WriteLine($"Rizzy.State: Error deserializing property '{prop.Name}' for type '{typeof(TViewModel).Name}'. JSON value kind: {jsonValue.ValueKind}. Exception: {ex.Message}");
                    }
                }
                else if (prop.PropertyType.IsClass || Nullable.GetUnderlyingType(prop.PropertyType) != null)
                {
                    // If the JSON value is null and the property is a reference type or nullable value type, set it to null.
                    prop.SetValue(viewModel, null);
                }
                // If JSON value is null and property is a non-nullable value type, it will retain its default value
                // (or throw if not properly initialized by the constructor, though Deserialize into an existing object helps).
            }
        }
    }
}