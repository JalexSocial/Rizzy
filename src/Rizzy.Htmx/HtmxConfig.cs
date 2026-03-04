using System.Text.Json.Serialization;

namespace Rizzy.Htmx;

/// <summary>
/// Htmx configuration options.
/// </summary>
public record class HtmxConfig
{
    internal class AntiForgeryConfiguration
    {
        [JsonPropertyName("formFieldName")]
        public string? FormFieldName { get; set; }
        [JsonPropertyName("headerName")]
        public string? HeaderName { get; set; }
        [JsonPropertyName("cookieName")]
        public string? CookieName { get; set; }
        [JsonPropertyName("requestToken")]
        public string? RequestToken { get; set; }
    }

    /// <summary>
    /// Defaults to <see langword="true" /> if this property is null. really only useful for testing
    /// </summary>
    [JsonPropertyName("history")]
    public bool? History { get; set; }

    /// <summary>
    /// Defaults to <see cref="SwapStyle.innerHTML"/> if this property is null.
    /// </summary>
    [JsonPropertyName("defaultSwap")]
    public SwapStyle? DefaultSwap { get; set; }

    /// <summary>
    /// Defaults to <see langword="1"/> if this property is null.
    /// </summary>
    [JsonPropertyName("defaultSettleDelay")]
    public TimeSpan? DefaultSettleDelay { get; set; }

    /// <summary>
    /// Defaults to <see langword="true" /> if this property is null.
    /// Determines if the indicator styles are loaded.
    /// </summary>
    [JsonPropertyName("includeIndicatorCSS")]
    public bool? IncludeIndicatorCSS { get; set; }

    /// <summary>
    /// Defaults to <c>htmx-indicator</c> if this property is null.
    /// </summary>
    [JsonPropertyName("indicatorClass")]
    public string? IndicatorClass { get; set; }

    /// <summary>
    /// Defaults to <c>htmx-request</c> if this property is null.
    /// </summary>
    [JsonPropertyName("requestClass")]
    public string? RequestClass { get; set; }


    /// <summary>
    /// Defaults to <c>''</c> (empty string) if this property is null,
    /// meaning that no nonce will be added to inline scripts
    /// </summary>
    [JsonPropertyName("inlineScriptNonce")]
    public string? InlineScriptNonce { get; set; }

    /// <summary>
    /// Defaults to <c>''</c> (empty string) if this property is null,
    /// meaning that no nonce will be added to inline styles
    /// </summary>
    [JsonPropertyName("inlineStyleNonce")]
    public string? InlineStyleNonce { get; set; }

    /// <summary>
    /// Defaults to <c>''</c> (empty string) if this property is null,
    /// meaning that no nonce has been defined for this document.  This is a Rizzy-specific setting
    /// intended to allow a nonce to verify against without deliberately injecting it into all
    /// htmx server responses
    /// </summary>
    [JsonInclude, JsonPropertyName("documentNonce")]
    internal string? DocumentNonce { get; set; }


    /// <summary>
    /// Defaults to <see langword="60000" /> if this property is null.
    /// The number of milliseconds a request can take before automatically being terminated
    /// </summary>
    [JsonPropertyName("defaultTimeout")]
    public TimeSpan? DefaultTimeout { get; set; }

    /// <summary>
    /// Defaults to <see langword="false" /> if this property is null. 
    /// If the focused element should be scrolled into view, and can be overridden using the focus-scroll swap modifier.
    /// </summary>
    [JsonPropertyName("defaultFocusScroll")]
    public bool? DefaultFocusScroll { get; set; }

    /// <summary>
    /// Defaults to <see langword="false" /> if this property is null.
    /// If set to <see langword="true" />, htmx will use the View Transition API when swapping in new content.
    /// </summary>
    [JsonPropertyName("transitions")]
    public bool? Transitions { get; set; }

    /// <summary>
    /// Controls cross-origin request mode. HTMX defaults this to <c>same-origin</c>.
    /// </summary>
    [JsonPropertyName("mode")]
    public string? Mode { get; set; }

    /// <summary>
    /// Enables implicit inheritance of htmx attributes when true.
    /// </summary>
    [JsonPropertyName("implicitInheritance")]
    public bool? ImplicitInheritance { get; set; }

    /// <summary>
    /// Status code ranges and values that should not be swapped.
    /// </summary>
    [JsonPropertyName("noSwap")]
    public object[]? NoSwap { get; set; }


    /// <summary>
    /// Attributes to ignore when morphing. Defaults to["data-htmx-powered"].
    /// </summary>
    [JsonPropertyName("morphIgnore")]
    public string[]? MorphIgnore { get; set; }

    /// <summary>
    /// Sibling scan limit during morphing. Defaults to 10.
    /// </summary>
    [JsonPropertyName("morphScanLimit")]
    public int? MorphScanLimit { get; set; }

    /// <summary>
    /// CSS selector for elements to skip morphing.
    /// </summary>
    [JsonPropertyName("morphSkip")]
    public string? MorphSkip { get; set; }

    /// <summary>
    /// CSS selector for elements whose children skip morphing.
    /// </summary>
    [JsonPropertyName("morphSkipChildren")]
    public string? MorphSkipChildren { get; set; }

    [JsonInclude, JsonPropertyName("antiforgery")]
    internal AntiForgeryConfiguration? Antiforgery { get; set; } = null;

    /// <summary>
    /// If set to true, will utilize an IRizzyNonceProvider instance to generate script nonces
    /// </summary>
    [JsonIgnore]
    public bool GenerateScriptNonce { get; set; } = false;

    /// <summary>
    /// If set to true, will utilize an IRizzyNonceProvider instance to generate script nonces
    /// </summary>
    [JsonIgnore]
    public bool GenerateStyleNonce { get; set; } = false;

    /// <summary>
    /// Whitelist of allowed extensions (empty = allow all).
    /// Required in HTMX 4 to restrict extension loading.
    /// </summary>
    [JsonPropertyName("extensions")]
    public string? Extensions { get; set; }

    /// <summary>
    /// Custom attribute prefix. HTMX 4 no longer supports <c>data-hx-</c> implicitly.
    /// If you wish to use <c>data-hx-</c>, set this to <c>"data-hx-"</c>.
    /// </summary>
    [JsonPropertyName("prefix")]
    public string? Prefix { get; set; }

}
