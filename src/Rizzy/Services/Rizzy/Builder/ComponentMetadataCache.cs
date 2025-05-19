using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace Rizzy.Internal;

/// <summary>
/// Provides a thread-safe cache for Blazor component parameter metadata. This class is for internal use.
/// </summary>
internal static class ComponentMetadataCache
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<ParameterMetadata>> Cache = new();
    // ComponentBase itself has no [Parameter] properties we need to reflect on for this builder's purpose.
    // We stop traversal when we hit ComponentBase or System.Object.
    private static readonly Type ComponentBaseTypeGlobal = typeof(ComponentBase);

    /// <summary>
    /// Retrieves the parameter metadata for a given component type.
    /// Metadata is calculated once per component type and then cached.
    /// </summary>
    /// <param name="componentType">The type of the Blazor component.</param>
    /// <returns>A read-only list of <see cref="ParameterMetadata"/> for the component type.</returns>
    public static IReadOnlyList<ParameterMetadata> GetParameterMetadata(Type componentType)
    {
        return Cache.GetOrAdd(componentType, CalculateMetadataForType);
    }

    private static IReadOnlyList<ParameterMetadata> CalculateMetadataForType(Type componentType)
    {
        var parameterList = new List<ParameterMetadata>();
        var namesEncountered = new HashSet<string>(StringComparer.Ordinal);

        Type? currentTypeInHierarchy = componentType;

        // Traverse the inheritance hierarchy up to (but not including) ComponentBase or System.Object
        while (currentTypeInHierarchy != null &&
               currentTypeInHierarchy != typeof(object) &&
               currentTypeInHierarchy != ComponentBaseTypeGlobal) // Stop if we reach ComponentBase
        {
            var properties = currentTypeInHierarchy.GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var propertyInfo in properties)
            {
                // Check for [ParameterAttribute] directly on the declared property
                var parameterAttribute = propertyInfo.GetCustomAttribute<ParameterAttribute>(inherit: false);
                if (parameterAttribute != null)
                {
                    // Only add if this parameter name hasn't been seen from a more derived type
                    // (ensures derived parameters shadow base if names collide, though Blazor itself might warn/error on this)
                    if (namesEncountered.Add(propertyInfo.Name))
                    {
                        var editorRequiredAttribute = propertyInfo.GetCustomAttribute<EditorRequiredAttribute>(inherit: false);
                        parameterList.Add(new ParameterMetadata(
                            propertyInfo.Name,
                            propertyInfo,
                            propertyInfo.PropertyType,
                            IsEditorRequired: editorRequiredAttribute != null
                        ));
                    }
                }
            }
            currentTypeInHierarchy = currentTypeInHierarchy.BaseType;
        }
        return parameterList.AsReadOnly();
    }
}
