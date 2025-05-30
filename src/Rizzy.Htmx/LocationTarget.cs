﻿using System.Text.Json.Serialization;

namespace Rizzy.Htmx;

public sealed class LocationTarget : AjaxContext
{
    /// <summary>
    /// Gets or sets the source element of the request.
    /// </summary>
    [JsonPropertyName("path")]
    public required string Path { get; set; }
}
