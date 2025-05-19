using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace Rizzy.Internal;

/// <summary>
/// Stores metadata about a Blazor component's parameter. This record is for internal use.
/// </summary>
/// <param name="Name">The name of the parameter.</param>
/// <param name="PropertyInfo">Reflection information for the parameter's property.</param>
/// <param name="ParameterType">The .NET type of the parameter.</param>
/// <param name="IsEditorRequired">Indicates if the parameter is marked with <see cref="EditorRequiredAttribute"/>.</param>
internal record ParameterMetadata(
	string Name,
	PropertyInfo PropertyInfo,
	Type ParameterType,
	bool IsEditorRequired
);
