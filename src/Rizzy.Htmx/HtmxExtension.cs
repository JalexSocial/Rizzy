namespace Rizzy.Htmx;

/// <summary>
/// An htmx 4 extension: its registered name (what <c>htmx.config.extensions</c> must list) and the
/// script file that registers it. Enable extensions with
/// <see cref="HtmxConfig.UseExtensions(HtmxExtension[])"/>; <c>&lt;RzScripts /&gt;</c> then loads
/// their scripts and Rizzy keeps the allow-list in step.
/// </summary>
/// <remarks>
/// The registered name often differs from the file name (<c>hx-sse.js</c> registers <c>sse</c>).
/// htmx silently rejects an extension whose registered name is missing from a non-empty
/// <c>htmx.config.extensions</c>, which is the mismatch this catalog prevents.
/// </remarks>
public sealed class HtmxExtension : IEquatable<HtmxExtension>
{
    private HtmxExtension(string name, string? fileName, string description)
    {
        Name = name;
        FileName = fileName;
        Description = description;
    }

    /// <summary>The name the extension registers with (<c>htmx.registerExtension(name, ...)</c>).</summary>
    public string Name { get; }

    /// <summary>
    /// The script file name without extension, relative to htmx's <c>dist/ext/</c> folder
    /// (for example <c>hx-sse</c>), or <see langword="null"/> for Rizzy's own extensions, which
    /// are bundled in <c>rizzy.js</c>.
    /// </summary>
    public string? FileName { get; }

    /// <summary>A one-line description.</summary>
    public string Description { get; }

    /// <summary>Whether the extension is bundled in <c>rizzy.js</c> rather than loaded from a script of its own.</summary>
    public bool IsBundledWithRizzy => FileName is null && !IsCustom;

    /// <summary>
    /// For a <see cref="Custom"/> extension: the URL of its script, which <c>&lt;RzScripts /&gt;</c>
    /// loads after htmx and Rizzy. <see langword="null"/> for official and Rizzy extensions.
    /// </summary>
    public string? ScriptUrl { get; private init; }

    /// <summary>
    /// Describes a third-party (or your own) extension, so it is added to the allow-list and, when
    /// <paramref name="scriptUrl"/> is given, loaded by <c>&lt;RzScripts /&gt;</c>.
    /// </summary>
    /// <param name="name">The name the extension registers with (<c>htmx.registerExtension(name, ...)</c>).</param>
    /// <param name="scriptUrl">Optional URL of the extension's script. Omit it if you load the script yourself.</param>
    /// <param name="description">Optional description.</param>
    public static HtmxExtension Custom(string name, string? scriptUrl = null, string description = "")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new HtmxExtension(name, null, description) { ScriptUrl = scriptUrl, IsCustom = true };
    }

    /// <summary>Whether this descriptor was created with <see cref="Custom"/>.</summary>
    public bool IsCustom { get; private init; }

    // ---- Official htmx 4.0.1 extensions (htmx.org/dist/ext) ------------------------------------

    /// <summary><c>hx-sse.js</c>: Server-Sent Events (<c>hx-sse:connect</c>, <c>hx-sse:close</c>, <c>sse:</c> triggers).</summary>
    public static readonly HtmxExtension Sse = new("sse", "hx-sse", "Server-Sent Events");
    /// <summary><c>hx-ws.js</c>: WebSockets (<c>hx-ws:connect</c>, <c>hx-ws:send</c>).</summary>
    public static readonly HtmxExtension Ws = new("ws", "hx-ws", "WebSockets");
    /// <summary><c>hx-preload.js</c>: preloads boosted links and <c>hx-preload</c> elements.</summary>
    public static readonly HtmxExtension Preload = new("preload", "hx-preload", "Preload requests before they are triggered");
    /// <summary><c>hx-head.js</c>: merges <c>&lt;head&gt;</c> elements from responses (<c>hx-head</c>, <c>hx-preserve</c>).</summary>
    public static readonly HtmxExtension Head = new("hx-head", "hx-head", "Head tag merging");
    /// <summary><c>hx-prompt.js</c>: htmx 2's <c>hx-prompt</c>; the answer is sent as <c>HX-Prompt</c>.</summary>
    public static readonly HtmxExtension Prompt = new("hx-prompt", "hx-prompt", "Prompt before a request (HX-Prompt)");
    /// <summary><c>hx-live.js</c>: reactive <c>hx-live</c> expressions.</summary>
    public static readonly HtmxExtension Live = new("hx-live", "hx-live", "Reactive live expressions");
    /// <summary><c>hx-pending.js</c>: shows a template (<c>hx-pending</c>) in the target while a request runs.</summary>
    public static readonly HtmxExtension Pending = new("hx-pending", "hx-pending", "Pending content while a request runs");
    /// <summary><c>hx-ptag.js</c>: polling tags (<c>hx-ptag</c>, <c>HX-PTag</c>) so unchanged content is skipped.</summary>
    public static readonly HtmxExtension PTag = new("ptag", "hx-ptag", "Polling tags (HX-PTag)");
    /// <summary><c>hx-targets.js</c>: swaps one response into every element matching <c>hx-targets</c>.</summary>
    public static readonly HtmxExtension Targets = new("hx-targets", "hx-targets", "Multiple swap targets");
    /// <summary><c>hx-browser-indicator.js</c>: shows the browser's own loading indicator (<c>hx-browser-indicator</c>).</summary>
    public static readonly HtmxExtension BrowserIndicator = new("browser-indicator", "hx-browser-indicator", "Browser loading indicator");
    /// <summary><c>hx-csp.js</c>: nonce-gated htmx processing (<c>hx-nonce</c>). An alternative to Rizzy's own <c>rizzy-nonce</c>.</summary>
    public static readonly HtmxExtension Csp = new("hx-csp", "hx-csp", "Nonce-gated htmx processing (hx-nonce)");
    /// <summary><c>hx-multipart.js</c>: streamed multipart responses (<c>hx-multipart:connect</c>, <c>HX-Last-Part-ID</c>).</summary>
    public static readonly HtmxExtension Multipart = new("hx-multipart", "hx-multipart", "Multipart response streaming");
    /// <summary><c>hx-upsert.js</c>: the <c>upsert</c> swap style.</summary>
    public static readonly HtmxExtension Upsert = new("upsert", "hx-upsert", "Upsert swap style");
    /// <summary><c>hx-download.js</c>: the <c>download</c> swap style and <c>HX-Download</c>.</summary>
    public static readonly HtmxExtension Download = new("download", "hx-download", "File downloads with progress (HX-Download)");
    /// <summary><c>hx-history-cache.js</c>: a localStorage history cache, as in htmx 2.</summary>
    public static readonly HtmxExtension HistoryCache = new("history-cache", "hx-history-cache", "History cache");
    /// <summary><c>htmx-2-compat.js</c>: htmx 2 event names and behavior.</summary>
    public static readonly HtmxExtension Htmx2Compat = new("compat", "htmx-2-compat", "htmx 2 compatibility");
    /// <summary><c>hx-alpine-compat.js</c>: initializes Alpine.js on swapped fragments and preserves its state when morphing.</summary>
    public static readonly HtmxExtension AlpineCompat = new("alpine-compat", "hx-alpine-compat", "Alpine.js compatibility");

    // ---- Rizzy's own extensions (bundled in rizzy.js) -------------------------------------------

    /// <summary>Adds the antiforgery token to unsafe htmx requests.</summary>
    public static readonly HtmxExtension RizzyAntiforgery = new("rizzy-antiforgery", null, "Antiforgery token for htmx requests");
    /// <summary>Lets server-rendered scripts in htmx responses run under a nonce-based CSP.</summary>
    public static readonly HtmxExtension RizzyNonce = new("rizzy-nonce", null, "CSP nonces for htmx responses");
    /// <summary>Blazor streaming rendering over htmx.</summary>
    public static readonly HtmxExtension RizzyStreaming = new("rizzy-streaming", null, "Blazor streaming rendering");
    /// <summary><c>hx-confirm</c> with the native dialog or a custom one.</summary>
    public static readonly HtmxExtension RizzyConfirm = new("rizzy-confirm", null, "hx-confirm dialogs");

    /// <summary>The official htmx 4 extensions, in alphabetical order of file name.</summary>
    public static IReadOnlyList<HtmxExtension> Official { get; } =
    [
        Htmx2Compat, AlpineCompat, BrowserIndicator, Csp, Download, Head, HistoryCache, Live,
        Multipart, Pending, Preload, Prompt, PTag, Sse, Targets, Upsert, Ws,
    ];

    /// <summary>Rizzy's own extensions, always loaded with <c>rizzy.js</c>.</summary>
    public static IReadOnlyList<HtmxExtension> Rizzy { get; } =
        [RizzyAntiforgery, RizzyNonce, RizzyStreaming, RizzyConfirm];

    /// <inheritdoc/>
    public bool Equals(HtmxExtension? other) => other is not null && string.Equals(Name, other.Name, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as HtmxExtension);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Name);

    /// <inheritdoc/>
    public override string ToString() => Name;
}
