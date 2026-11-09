using System.Text.Json.Serialization;

namespace Rizzy.Htmx;

/// <summary>
/// Options for an htmx 4 ajax request, matching the shape of the <c>htmx.ajax(verb, path, options)</c>
/// options object. Used as the JSON body of the <c>HX-Location</c> response header.
/// </summary>
/// <remarks>
/// htmx copies these options onto its request context, so property names and value types follow
/// htmx 4.0.1 exactly: <see cref="Values"/> and <see cref="Headers"/> are JSON objects, not strings.
/// </remarks>
public class AjaxContext
{
    /// <summary>
    /// Gets or sets a CSS selector for the source element of the request (defaults to <c>body</c>).
    /// </summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets the name of an event that "triggered" the request.
    /// </summary>
    [JsonPropertyName("event")]
    public string? Event { get; set; }

    /// <summary>
    /// Gets or sets the target to swap the response into.
    /// </summary>
    [JsonPropertyName("target")]
    public string? Target { get; set; }

    /// <summary>
    /// Gets or sets how the response will be swapped in relative to the target (an <c>hx-swap</c> value).
    /// </summary>
    [JsonPropertyName("swap")]
    public string? Swap { get; set; }

    /// <summary>
    /// Gets or sets values to submit with the request, as name/value pairs.
    /// </summary>
    [JsonPropertyName("values")]
    public IDictionary<string, string>? Values { get; set; }

    /// <summary>
    /// Gets or sets headers to submit with the request. Header names are sent exactly as given.
    /// </summary>
    [JsonPropertyName("headers")]
    public IDictionary<string, string>? Headers { get; set; }

    /// <summary>
    /// Gets or sets a CSS selector choosing the content to swap from the response.
    /// </summary>
    [JsonPropertyName("select")]
    public string? Select { get; set; }

    /// <summary>
    /// Gets or sets out-of-band selections from the response (an <c>hx-select-oob</c> value).
    /// </summary>
    [JsonPropertyName("selectOOB")]
    public string? SelectOob { get; set; }

    /// <summary>
    /// Gets or sets the history push behaviour: <c>"true"</c>, <c>"false"</c> or a URL. When neither
    /// <see cref="Push"/> nor <see cref="Replace"/> is set, htmx pushes the location path.
    /// </summary>
    [JsonPropertyName("push")]
    public string? Push { get; set; }

    /// <summary>
    /// Gets or sets the history replace behaviour: <c>"true"</c>, <c>"false"</c> or a URL.
    /// </summary>
    [JsonPropertyName("replace")]
    public string? Replace { get; set; }

    /// <summary>
    /// Gets or sets whether the swap uses the View Transitions API.
    /// </summary>
    [JsonPropertyName("transition")]
    public bool? Transition { get; set; }
}
