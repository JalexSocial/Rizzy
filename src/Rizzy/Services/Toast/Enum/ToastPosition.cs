using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Rizzy;

/// <summary>
/// Represents the possible screen positions for a toast notification.
/// Corresponds to simple-notify 'position' option.
/// </summary>
[JsonConverter(typeof(EnumMemberJsonConverter<ToastPosition>))]
public enum ToastPosition
{
    /// <summary>
    /// Positions the notification in the top right corner.
    /// </summary>
    [EnumMember(Value = "right top")]
    RightTop,
    /// <summary>
    /// Alias for RightTop. Positions the notification in the top right corner.
    /// </summary>
    [EnumMember(Value = "top right")]
    TopRight,

    /// <summary>
    /// Positions the notification in the bottom right corner.
    /// </summary>
    [EnumMember(Value = "right bottom")]
    RightBottom,
    /// <summary>
    /// Alias for RightBottom. Positions the notification in the bottom right corner.
    /// </summary>
    [EnumMember(Value = "bottom right")]
    BottomRight,

    /// <summary>
    /// Positions the notification in the top left corner.
    /// </summary>
    [EnumMember(Value = "left top")]
    LeftTop,
    /// <summary>
    /// Alias for LeftTop. Positions the notification in the top left corner.
    /// </summary>
    [EnumMember(Value = "top left")]
    TopLeft,

    /// <summary>
    /// Positions the notification in the bottom left corner.
    /// </summary>
    [EnumMember(Value = "left bottom")]
    LeftBottom,
    /// <summary>
    /// Alias for LeftBottom. Positions the notification in the bottom left corner.
    /// </summary>
    [EnumMember(Value = "bottom left")]
    BottomLeft,

    /// <summary>
    /// Positions the notification centered horizontally at the top edge.
    /// </summary>
    [EnumMember(Value = "center top")]
    CenterTop,
    /// <summary>
    /// Alias for CenterTop. Positions the notification centered horizontally at the top edge.
    /// </summary>
    [EnumMember(Value = "x-center top")]
    XCenterTop,

    /// <summary>
    /// Positions the notification centered horizontally at the bottom edge.
    /// </summary>
    [EnumMember(Value = "center bottom")]
    CenterBottom,
    /// <summary>
    /// Alias for CenterBottom. Positions the notification centered horizontally at the bottom edge.
    /// </summary>
    [EnumMember(Value = "x-center bottom")]
    XCenterBottom,

    /// <summary>
    /// Assumed alias for LeftYCenter. Positions the notification centered vertically on the left edge.
    /// </summary>
    [EnumMember(Value = "left center")]
    LeftCenter,
    /// <summary>
    /// Positions the notification centered vertically on the left edge.
    /// </summary>
    [EnumMember(Value = "left y-center")]
    LeftYCenter,
    /// <summary>
    /// Alias for LeftYCenter. Positions the notification centered vertically on the left edge.
    /// </summary>
    [EnumMember(Value = "y-center left")]
    YCenterLeft,

    /// <summary>
    /// Assumed alias for RightYCenter. Positions the notification centered vertically on the right edge.
    /// </summary>
    [EnumMember(Value = "right center")]
    RightCenter,
    /// <summary>
    /// Positions the notification centered vertically on the right edge.
    /// </summary>
    [EnumMember(Value = "right y-center")]
    RightYCenter,
    /// <summary>
    /// Alias for RightYCenter. Positions the notification centered vertically on the right edge.
    /// </summary>
    [EnumMember(Value = "y-center right")]
    YCenterRight,

    /// <summary>
    /// Assumed alias for TopXCenter. Positions the notification centered horizontally at the top edge.
    /// </summary>
    [EnumMember(Value = "top center")]
    TopCenter,
    /// <summary>
    /// Positions the notification centered horizontally at the top edge.
    /// </summary>
    [EnumMember(Value = "top x-center")]
    TopXCenter,
    // x-center top already covered by XCenterTop

    /// <summary>
    /// Assumed alias for BottomXCenter. Positions the notification centered horizontally at the bottom edge.
    /// </summary>
    [EnumMember(Value = "bottom center")]
    BottomCenter,
    /// <summary>
    /// Positions the notification centered horizontally at the bottom edge.
    /// </summary>
    [EnumMember(Value = "bottom x-center")]
    BottomXCenter,
    // x-center bottom already covered by XCenterBottom

    /// <summary>
    /// Positions the notification in the absolute center of the screen (both horizontally and vertically).
    /// </summary>
    [EnumMember(Value = "center")]
    Center
}
