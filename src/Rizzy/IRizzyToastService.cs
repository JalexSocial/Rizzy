namespace Rizzy;

/// <summary>
/// Toast service interface
/// </summary>
public interface IRizzyToastService
{
	/// <summary>
	/// Adds a success toast notification.
	/// </summary>
	/// <param name="text">The main body text of the notification.</param>
	/// <param name="title">Optional title. If null, a default title "Success" will be used.</param>
	/// <param name="options">Optional configuration settings to override defaults.</param>
	void Success(string text, string? title = null, ToastMessageOptions? options = null);

	/// <summary>
	/// Adds a warning toast notification.
	/// </summary>
	/// <param name="text">The main body text of the notification.</param>
	/// <param name="title">Optional title. If null, a default title "Warning" will be used.</param>
	/// <param name="options">Optional configuration settings to override defaults.</param>
	void Warning(string text, string? title = null, ToastMessageOptions? options = null);

	/// <summary>
	/// Adds an information toast notification.
	/// </summary>
	/// <param name="text">The main body text of the notification.</param>
	/// <param name="title">Optional title. If null, a default title "Info" will be used.</param>
	/// <param name="options">Optional configuration settings to override defaults.</param>
	void Information(string text, string? title = null, ToastMessageOptions? options = null);

	/// <summary>
	/// Adds an error toast notification.
	/// </summary>
	/// <param name="text">The main body text of the notification.</param>
	/// <param name="title">Optional title. If null, a default title "Error" will be used.</param>
	/// <param name="options">Optional configuration settings to override defaults.</param>
	void Error(string text, string? title = null, ToastMessageOptions? options = null);

	/// <summary>
	/// Adds a custom pre-configured toast notification directly to the list.
	/// Ensures required properties like Text are set.
	/// </summary>
	/// <param name="notification">The <see cref="ToastMessage"/> object containing all configuration for the toast. `Text` property is required.</param>
	/// <exception cref="ArgumentNullException">Thrown if notification is null.</exception>
	/// <exception cref="ArgumentException">Thrown if notification.Text is null or empty.</exception>
	void Custom(ToastMessage notification);

	/// <summary>
	/// Retrieves all currently pending toast notifications.
	/// Returns a *copy* of the internal list to prevent external modification.
	/// </summary>
	/// <returns>A new list containing <see cref="ToastMessage"/> objects.</returns>
	List<ToastMessage> ReadAllNotifications();

	/// <summary>
	/// Removes all currently pending toast notifications from the service.
	/// </summary>
	void RemoveAll();
}