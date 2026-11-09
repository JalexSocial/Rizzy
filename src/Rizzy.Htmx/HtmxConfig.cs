using System.Text.Json.Serialization;
using Rizzy.Htmx.Serialization;

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
        [JsonPropertyName("requestToken")]
        public string? RequestToken { get; set; }
    }

    /// <summary>
    /// Defaults to <see langword="true" /> if this property is null. really only useful for testing
    /// </summary>
    [JsonPropertyName("history")]
    [JsonConverter(typeof(HtmxHistoryModeJsonConverter))]
    public HtmxHistoryMode? History { get; set; }

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
    /// A nonce htmx stamps on <em>every</em> script it inserts from a response. Leave this unset when
    /// using Rizzy's nonce extension, which promotes only scripts the server rendered with the
    /// response nonce; setting it also trusts any script injected into a response.
    /// </summary>
    [JsonPropertyName("inlineScriptNonce")]
    public string? InlineScriptNonce { get; set; }


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
    /// When true, sets <see cref="InlineScriptNonce"/> to the page's <c>IRizzyNonceProvider</c>
    /// nonce, so htmx stamps it on every script in every response.
    /// </summary>
    /// <remarks>
    /// Not recommended. Rizzy's nonce extension already promotes scripts rendered with the response
    /// nonce (the <c>HX-Nonce</c> header) to the page nonce, while injected scripts stay blocked
    /// by the CSP. This option trusts all response scripts and writes the nonce into the
    /// <c>htmx-config</c> meta tag, where CSS attribute selectors can read it.
    /// </remarks>
    [JsonIgnore]
    public bool GenerateScriptNonce { get; set; } = false;

    /// <summary>
    /// Whitelist of allowed extensions (empty = allow all).
    /// Required in HTMX 4 to restrict extension loading.
    /// </summary>
    [JsonPropertyName("extensions")]
    public string? Extensions { get; set; }

    /// <summary>
    /// An additional attribute prefix that htmx accepts alongside <c>hx-</c>. htmx 4 defaults to
    /// <c>"data-hx-"</c>, so both <c>hx-get</c> and <c>data-hx-get</c> work out of the box. Set a
    /// different value (for example <c>"x-hx-"</c>) to accept that prefix instead of
    /// <c>data-hx-</c>, or an empty string to accept only <c>hx-</c>.
    /// </summary>
    [JsonPropertyName("prefix")]
    public string? Prefix { get; set; }

    /// <summary>
    /// Enables logging of all htmx lifecycle events to the browser console.
    /// Useful for debugging htmx behavior.
    /// </summary>
    [JsonPropertyName("logAll")]
    public bool? LogAll { get; set; }

    /// <summary>
    /// When true, an empty main response is still swapped after out-of-band or partial content was
    /// processed. htmx 4 defaults to <c>false</c>.
    /// </summary>
    [JsonPropertyName("allowEmptySwapAfterOOB")]
    public bool? AllowEmptySwapAfterOOB { get; set; }

    /// <summary>
    /// A character htmx uses instead of <c>:</c> in attribute and event names (for example
    /// <c>"-"</c> to write <c>hx-on-click</c> and <c>hx-status-422</c>), for templating systems that
    /// do not allow colons in attribute names. htmx 4 defaults to <c>:</c>.
    /// </summary>
    [JsonPropertyName("metaCharacter")]
    public string? MetaCharacter { get; set; }

    /// <summary>
    /// The Rizzy server version, emitted so the client bundle can detect a server/client mismatch.
    /// Set automatically by <c>HtmxConfigHeadOutlet</c>; not read by htmx itself.
    /// </summary>
    [JsonInclude, JsonPropertyName("rizzyVersion")]
    internal string? RizzyVersion { get; set; }

}
