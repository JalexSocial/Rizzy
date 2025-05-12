using System.Text.Json; // For JsonElement

namespace Rizzy.State.Serialization;

/// <summary>
/// Defines methods for extracting and populating view model state
/// based on properties marked with specific attributes (e.g., RizzyStateAttribute).
/// </summary>
public interface IRizzyViewModelSerializer
{
	/// <summary>
	/// Extracts properties marked for persistence from a view model instance.
	/// </summary>
	/// <typeparam name="TViewModel">The type of the view model.</typeparam>
	/// <param name="viewModel">The view model instance.</param>
	/// <returns>A dictionary where keys are property names (typically camelCased for JSON)
	/// and values are the corresponding property values.</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="viewModel"/> is null.</exception>
	IDictionary<string, object?> ExtractPersistentState<TViewModel>(TViewModel viewModel) where TViewModel : class;

	/// <summary>
	/// Populates properties marked for persistence on a view model instance from a dictionary of state.
	/// </summary>
	/// <typeparam name="TViewModel">The type of the view model.</typeparam>
	/// <param name="viewModel">The view model instance to populate.</param>
	/// <param name="persistentState">A dictionary where keys are property names (expected to be camelCased from JSON)
	/// and values are <see cref="JsonElement"/> representing the property values.</param>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="viewModel"/> or <paramref name="persistentState"/> is null.</exception>
	void PopulateViewModel<TViewModel>(TViewModel viewModel, IDictionary<string, JsonElement> persistentState) where TViewModel : class;
}