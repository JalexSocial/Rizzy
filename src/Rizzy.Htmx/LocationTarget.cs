using System.Text.Json.Serialization;

namespace Rizzy.Htmx;

/// <summary>
/// The JSON form of the <c>HX-Location</c> response header: a path plus <see cref="AjaxContext"/>
/// options for the client-side <c>htmx.ajax('GET', path, options)</c> call.
/// </summary>
public sealed class LocationTarget : AjaxContext
{
    /// <summary>
    /// Gets or sets the path to request.
    /// </summary>
    [JsonPropertyName("path")]
    public required string Path { get; set; }
}
