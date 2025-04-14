using System.Text.Json.Serialization;

namespace Rizzy;

/// <summary>
/// Represents optional configuration settings for a toast message.
/// All properties are nullable; if a property is null, the default behavior
/// of the underlying notification library (simple-notify) will be used.
/// </summary>
public record class ToastMessageOptions
{
    /// <summary>
    /// Custom HTML string for the icon area. If set, overrides the default icon.
    /// </summary>
    [JsonPropertyName("customIcon")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CustomIcon { get; init; }

    /// <summary>
    /// Custom CSS class name(s) to add to the notification container.
    /// </summary>
    [JsonPropertyName("customClass")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CustomClass { get; init; }

    /// <summary>
    /// The visual effect used for showing/hiding the notification (e.g., Fade, Slide).
    /// If null, the library default is used.
    /// </summary>
    [JsonPropertyName("effect")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ToastEffect? Effect { get; init; }

    /// <summary>
    /// The duration of the show/hide effect animation in milliseconds.
    /// If null, the library default is used.
    /// </summary>
    [JsonPropertyName("speed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Speed { get; init; }

    /// <summary>
    /// Whether to display the default status icon.
    /// If null, the library default is used (typically true).
    /// </summary>
    [JsonPropertyName("showIcon")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? ShowIcon { get; init; }

    /// <summary>
    /// Whether to display the close button.
    /// If null, the library default is used (typically true).
    /// </summary>
    [JsonPropertyName("showCloseButton")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? ShowCloseButton { get; init; }

    /// <summary>
    /// Whether the notification should close automatically after a timeout.
    /// If null, the library default is used (typically true).
    /// </summary>
    [JsonPropertyName("autoclose")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AutoClose { get; init; }

    /// <summary>
    /// The time in milliseconds before the notification automatically closes.
    /// Only applicable if AutoClose is true or default.
    /// If null, the library default is used (typically 4000).
    /// </summary>
    [JsonPropertyName("autotimeout")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? AutoTimeout { get; init; }

    /// <summary>
    /// The position on the screen where the notification should appear.
    /// If null, the library default is used (typically RightTop).
    /// </summary>
    [JsonPropertyName("position")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ToastPosition? Position { get; init; }

    /// <summary>
    /// The margin (in pixels) between multiple notifications stacked in the same position.
    /// If null, the library default is used (typically 20px).
    /// </summary>
    [JsonPropertyName("gap")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? Gap { get; init; }

    /// <summary>
    /// The distance (in pixels) from the screen edges.
    /// If null, the library default is used (typically 20px).
    /// </summary>
    [JsonPropertyName("distance")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? Distance { get; init; }

    /// <summary>
    /// The visual style type of the notification ('outline' or 'filled').
    /// If null, the library default is used (typically Outline).
    /// </summary>
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ToastType? Type { get; init; }
}