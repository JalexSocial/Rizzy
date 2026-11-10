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

    /// <summary>
    /// Set to <c>false</c> to silence the console warnings rizzy.js logs for a misconfigured page
    /// (missing or invalid meta config, or a server/client major version mismatch). Errors are
    /// still logged. Default: enabled.
    /// </summary>
    [JsonPropertyName("rizzyDiagnostics")]
    public bool? RizzyDiagnostics { get; set; }

    // ---- Extensions -----------------------------------------------------------------------------

    private List<HtmxExtension> _enabledExtensions = [];

    /// <summary>
    /// The extensions enabled with <see cref="UseExtensions"/>. <c>&lt;RzScripts /&gt;</c> loads their
    /// scripts, and Rizzy adds their registered names to the allow-list when one is in use.
    /// </summary>
    [JsonIgnore]
    public IReadOnlyList<HtmxExtension> EnabledExtensions => _enabledExtensions;

    /// <summary>
    /// Enables htmx extensions: <c>&lt;RzScripts /&gt;</c> loads their scripts after htmx, and their
    /// registered names are kept in the <c>htmx.config.extensions</c> allow-list whenever one is in
    /// use (see <see cref="RestrictExtensions"/>). Repeated calls add to the list.
    /// </summary>
    /// <example><code>
    /// builder.Services.AddHtmx(o => o.UseExtensions(HtmxExtension.Sse, HtmxExtension.Preload));
    /// </code></example>
    public HtmxConfig UseExtensions(params HtmxExtension[] extensions)
    {
        ArgumentNullException.ThrowIfNull(extensions);
        // A copy, so a config created with `with` never shares its list with the original.
        var list = new List<HtmxExtension>(_enabledExtensions);
        foreach (var extension in extensions)
        {
            ArgumentNullException.ThrowIfNull(extension);
            if (extension.IsBundledWithRizzy)
                continue; // always loaded with rizzy.js
            if (!list.Contains(extension))
                list.Add(extension);
        }
        _enabledExtensions = list;
        return this;
    }

    /// <summary>
    /// When <see langword="true"/>, htmx only accepts the extensions enabled with
    /// <see cref="UseExtensions"/>, any listed in <see cref="Extensions"/>, and (with the Rizzy
    /// package) Rizzy's own. Default <see langword="false"/>: htmx accepts every extension that
    /// registers, unless you set <see cref="Extensions"/> yourself; the enabled extensions (and
    /// Rizzy's own) are then added to it.
    /// </summary>
    [JsonIgnore]
    public bool RestrictExtensions { get; set; }

    /// <summary>How <c>&lt;RzScripts /&gt;</c> loads htmx, its extensions and the Rizzy script.</summary>
    [JsonIgnore]
    public HtmxScriptOptions Scripts { get; set; } = new();

    /// <summary>
    /// The extensions the server expects to register, so rizzy.js can warn about one that did not.
    /// Set automatically by <c>HtmxConfigHeadOutlet</c>.
    /// </summary>
    [JsonInclude, JsonPropertyName("rizzyExtensions")]
    internal string[]? ExpectedExtensions { get; set; }

    /// <summary>
    /// Returns the <c>htmx.config.extensions</c> value to write: <see langword="null"/> (all
    /// extensions allowed) unless an allow-list is in use, in which case it contains the listed
    /// names, <paramref name="alwaysAllowed"/> (Rizzy passes its own extensions) and every enabled
    /// extension.
    /// </summary>
    internal string? ResolveExtensionAllowList(IEnumerable<string>? alwaysAllowed = null)
    {
        var userList = (Extensions ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (!RestrictExtensions && userList.Length == 0)
            return null;

        var names = new List<string>();
        void Add(string name) { if (!names.Contains(name, StringComparer.Ordinal)) names.Add(name); }
        foreach (var name in userList) Add(name);
        foreach (var name in alwaysAllowed ?? []) Add(name);
        foreach (var extension in _enabledExtensions) Add(extension.Name);
        return string.Join(", ", names);
    }

    // ---- Extension settings ---------------------------------------------------------------------

    /// <summary>JSON keys written by HtmxConfig's own properties, mapped to the property name.</summary>
    private static readonly Lazy<Dictionary<string, string>> KnownKeys = new(() =>
        typeof(HtmxConfig)
            .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Select(p => (Property: p.Name, Json: p.GetCustomAttributes(typeof(JsonPropertyNameAttribute), true)
                .OfType<JsonPropertyNameAttribute>().FirstOrDefault()?.Name))
            .Where(p => p.Json is not null)
            .ToDictionary(p => p.Json!, p => p.Property, StringComparer.OrdinalIgnoreCase));

    /// <summary>
    /// Additional <c>htmx.config</c> keys, written into the meta tag as-is. Use <see cref="Set"/> to
    /// add them; extensions read their own sections (<c>sse</c>, <c>ws</c>, <c>preload</c>,
    /// <c>historyCache</c>, <c>live</c>, <c>multipart</c>, <c>boostBrowserIndicator</c>,
    /// <c>safeEval</c>, ...).
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, System.Text.Json.JsonElement> AdditionalSettings { get; set; } = new();

    /// <summary>
    /// Sets an additional <c>htmx.config</c> key, typically an extension's settings. The value is
    /// serialized as JSON with camelCase property names. Keys that an <see cref="HtmxConfig"/>
    /// property already writes (<c>mode</c>, <c>extensions</c>, ...) are rejected: set the property.
    /// </summary>
    /// <example><code>
    /// o.Set("sse", new { reconnect = true, reconnectDelay = 1000 })
    ///  .Set("historyCache", new { size = 20 });
    /// </code></example>
    public HtmxConfig Set(string key, object? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        if (KnownKeys.Value.TryGetValue(key, out var property))
            throw new ArgumentException(
                $"\"{key}\" is already written by HtmxConfig.{property}; set that property instead.", nameof(key));
        var settings = new Dictionary<string, System.Text.Json.JsonElement>(AdditionalSettings)
        {
            [key] = System.Text.Json.JsonSerializer.SerializeToElement(value, System.Text.Json.JsonSerializerOptions.Web),
        };
        AdditionalSettings = settings;
        return this;
    }

}
