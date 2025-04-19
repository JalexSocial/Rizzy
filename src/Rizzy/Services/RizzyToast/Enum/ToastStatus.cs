using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Rizzy;

/// <summary>
/// Represents the status types for a toast notification.
/// Corresponds to simple-notify 'status' option.
/// </summary>
[JsonConverter(typeof(EnumMemberJsonConverter<ToastStatus>))]
public enum ToastStatus
{
    /// <summary>
    /// Success notification (usually green).
    /// </summary>
    [EnumMember(Value = "success")]
    Success,

    /// <summary>
    /// Error notification (usually red).
    /// </summary>
    [EnumMember(Value = "error")]
    Error,

    /// <summary>
    /// Warning notification (usually orange/yellow).
    /// </summary>
    [EnumMember(Value = "warning")]
    Warning,

    /// <summary>
    /// Informational notification (usually blue).
    /// </summary>
    [EnumMember(Value = "info")]
    Info
}