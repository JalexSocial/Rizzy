using System; 
using System.Collections.Generic; 

namespace Rizzy; 

/// <summary>
/// An implementation that stores toast notifications in an in-memory list
/// and provides methods to create standard toast types.
/// </summary>
public class ToastService : IToastService
{
    /// <summary>
    /// Internal list holding the pending toast notifications.
    /// </summary>
    private readonly List<ToastMessage> _notifications = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ToastService"/> class.
    /// </summary>
    public ToastService() { }

    /// <summary>
    /// Internal helper to get a default title based on status.
    /// </summary>
    private static string GetDefaultTitle(ToastStatus status)
    {
        return status switch
        {
            ToastStatus.Success => "Success",
            ToastStatus.Error => "Error",
            ToastStatus.Warning => "Warning",
            ToastStatus.Info => "Info",
            _ => "Notification", // Fallback default
        };
    }

    /// <summary>
    /// Internal helper to create and add a toast, applying optional overrides.
    /// </summary>
    private void AddToast(ToastStatus status, string title, string text, ToastMessageOptions? options)
    {
        // Create the base message
        var message = new ToastMessage
        {
            Status = status,
            Title = title, // Use the provided or defaulted title
            Text = text   // Text is required by ToastMessage now
        };

        // Apply overrides from options if provided
        if (options != null)
        {
            message = message with
            {
                // Only override if the option property has a value
                CustomIcon = options.CustomIcon ?? message.CustomIcon,
                CustomClass = options.CustomClass ?? message.CustomClass,
                Effect = options.Effect ?? message.Effect,
                Speed = options.Speed ?? message.Speed,
                ShowIcon = options.ShowIcon ?? message.ShowIcon,
                ShowCloseButton = options.ShowCloseButton ?? message.ShowCloseButton,
                AutoClose = options.AutoClose ?? message.AutoClose,
                AutoTimeout = options.AutoTimeout ?? message.AutoTimeout,
                Position = options.Position ?? message.Position,
                Gap = options.Gap ?? message.Gap,
                Distance = options.Distance ?? message.Distance,
                Type = options.Type ?? message.Type
            };
        }

        _notifications.Add(message);
    }


    /// <summary>
    /// Adds a success toast notification.
    /// </summary>
    /// <param name="text">The main body text of the notification.</param>
    /// <param name="title">Optional title. If null, a default title "Success" will be used.</param>
    /// <param name="options">Optional configuration settings to override defaults.</param>
    public void Success(string text, string? title = null, ToastMessageOptions? options = null) =>
        AddToast(ToastStatus.Success, title ?? GetDefaultTitle(ToastStatus.Success), text, options);

    /// <summary>
    /// Adds a warning toast notification.
    /// </summary>
    /// <param name="text">The main body text of the notification.</param>
    /// <param name="title">Optional title. If null, a default title "Warning" will be used.</param>
    /// <param name="options">Optional configuration settings to override defaults.</param>
    public void Warning(string text, string? title = null, ToastMessageOptions? options = null) =>
        AddToast(ToastStatus.Warning, title ?? GetDefaultTitle(ToastStatus.Warning), text, options);

    /// <summary>
    /// Adds an information toast notification.
    /// </summary>
    /// <param name="text">The main body text of the notification.</param>
    /// <param name="title">Optional title. If null, a default title "Info" will be used.</param>
    /// <param name="options">Optional configuration settings to override defaults.</param>
    public void Information(string text, string? title = null, ToastMessageOptions? options = null) =>
        AddToast(ToastStatus.Info, title ?? GetDefaultTitle(ToastStatus.Info), text, options);

    /// <summary>
    /// Adds an error toast notification.
    /// </summary>
    /// <param name="text">The main body text of the notification.</param>
    /// <param name="title">Optional title. If null, a default title "Error" will be used.</param>
    /// <param name="options">Optional configuration settings to override defaults.</param>
    public void Error(string text, string? title = null, ToastMessageOptions? options = null) =>
        AddToast(ToastStatus.Error, title ?? GetDefaultTitle(ToastStatus.Error), text, options);

    /// <summary>
    /// Adds a custom pre-configured toast notification directly to the list.
    /// Ensures required properties like Text are set.
    /// </summary>
    /// <param name="notification">The <see cref="ToastMessage"/> object containing all configuration for the toast. `Text` property is required.</param>
    /// <exception cref="ArgumentNullException">Thrown if notification is null.</exception>
    /// <exception cref="ArgumentException">Thrown if notification.Text is null or empty.</exception>
    public void Custom(ToastMessage notification)
    {
        ArgumentNullException.ThrowIfNull(notification);
        // Add validation for the required Text property
        if (string.IsNullOrEmpty(notification.Text))
        {
             throw new ArgumentException("The 'Text' property of the notification cannot be null or empty.", nameof(notification));
        }
        // Ensure title defaults if not provided
        var finalNotification = notification with { Title = notification.Title ?? GetDefaultTitle(notification.Status) };

        _notifications.Add(finalNotification);
    }

    /// <summary>
    /// Retrieves all currently pending toast notifications.
    /// Returns a *copy* of the internal list to prevent external modification.
    /// </summary>
    /// <returns>A new list containing <see cref="ToastMessage"/> objects.</returns>
    public List<ToastMessage> ReadAllNotifications()
    {
        return [.._notifications]; // Return a copy
    }

    /// <summary>
    /// Removes all currently pending toast notifications from the service.
    /// </summary>
    public void RemoveAll()
    {
        _notifications.Clear();
    }
}