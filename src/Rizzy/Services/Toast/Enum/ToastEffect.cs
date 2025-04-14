using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Rizzy;

/// <summary>
/// Represents the visual effect used for showing/hiding the toast notification.
/// Corresponds to simple-notify 'effect' option.
/// </summary>
[JsonConverter(typeof(EnumMemberJsonConverter<ToastEffect>))]
public enum ToastEffect
{
    /// <summary>
    /// Fade in/out effect.
    /// </summary>
    [EnumMember(Value = "fade")]
    Fade,

    /// <summary>
    /// Slide in/out effect.
    /// </summary>
    [EnumMember(Value = "slide")]
    Slide
}