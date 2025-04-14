using System.Text.Json.Serialization;

namespace Rizzy;

/// <summary>
/// Represents a toast notification message to be displayed to the user.
/// Includes required content (<see cref="Text"/>), status, an optional title,
/// and inherits optional configuration settings from <see cref="ToastMessageOptions"/>.
/// Instances are typically created using object initializers or the 'with' expression.
/// </summary>
/// <remarks>
/// This record is designed to be serialized to JSON for use with a client-side notification library like simple-notify.
/// </remarks>
public sealed record class ToastMessage : ToastMessageOptions
{
	/// <summary>
	/// Gets the status of the notification (e.g., Success, Error, Info, Warning).
	/// This determines the visual styling (color, icon) of the toast.
	/// Defaults to <see cref="ToastStatus.Info"/>. This property is non-nullable.
	/// </summary>
	[JsonPropertyName("status")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] // Default is Info
	public ToastStatus Status { get; init; } = ToastStatus.Info;

	/// <summary>
	/// Gets the optional main title text displayed at the top of the notification.
	/// If left null when the toast is created via <see cref="ToastService"/> methods (e.g., `Success`, `Error`),
	/// a default title corresponding to the <see cref="Status"/> will be automatically assigned by the service.
	/// </summary>
	[JsonPropertyName("title")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? Title { get; init; }

	/// <summary>
	/// Gets the main body text of the notification. This property is required and must be provided
	/// when creating a <see cref="ToastMessage"/> instance.
	/// </summary>
	[JsonPropertyName("text")]
	public required string Text { get; init; }
}