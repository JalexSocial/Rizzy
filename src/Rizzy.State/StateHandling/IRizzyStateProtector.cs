using System.Diagnostics.CodeAnalysis;

namespace Rizzy.State.StateHandling;

/// <summary>
/// Defines methods for protecting and unprotecting view model state.
/// Protection typically involves serialization, compression, encryption, and signing.
/// </summary>
public interface IRizzyStateProtector
{
	/// <summary>
	/// Protects the given view model and its version into a secure string token.
	/// </summary>
	/// <typeparam name="TViewModel">The type of the view model.</typeparam>
	/// <param name="model">The view model instance to protect.</param>
	/// <param name="version">The version of the view model state.</param>
	/// <returns>A protected string token representing the view model state and version.</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="model"/> is null.</exception>
	/// <exception cref="StateProtectionException">Thrown if protection fails for any reason.</exception>
	string Protect<TViewModel>(TViewModel model, ulong version) where TViewModel : class;

	/// <summary>
	/// Attempts to unprotect a string token back into its original view model and version.
	/// </summary>
	/// <typeparam name="TViewModel">The expected type of the view model.</typeparam>
	/// <param name="token">The protected string token to unprotect.</param>
	/// <param name="model">When this method returns, contains the unprotected view model if unprotection was successful, or <c>null</c> otherwise.</param>
	/// <param name="version">When this method returns, contains the version of the unprotected view model state if unprotection was successful, or 0 otherwise.</param>
	/// <returns><c>true</c> if the token was successfully unprotected and deserialized; otherwise, <c>false</c>.</returns>
	bool TryUnprotect<TViewModel>(string token, [NotNullWhen(true)] out TViewModel? model, out ulong version) where TViewModel : class;
}