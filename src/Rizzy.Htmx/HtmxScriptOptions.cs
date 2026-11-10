namespace Rizzy.Htmx;

/// <summary>
/// Where htmx and its extension scripts are loaded from (used by Rizzy's <c>&lt;RzScripts /&gt;</c>).
/// Set through <see cref="HtmxConfig.Scripts"/>.
/// </summary>
public sealed record class HtmxScriptOptions
{
    /// <summary>
    /// The URL of htmx's <c>dist/</c> folder, ending in <c>/</c>. <c>htmx.min.js</c> and
    /// <c>ext/&lt;name&gt;.min.js</c> are loaded from here. Defaults to jsDelivr for the htmx version
    /// Rizzy is tested with (<see cref="HtmxAssets.Version"/>). Point it at your own copy (for
    /// example <c>/lib/htmx/</c>) to serve htmx yourself.
    /// </summary>
    public string BaseUrl { get; set; } = HtmxAssets.CdnBaseUrl;

    /// <summary>Load the minified files (<c>htmx.min.js</c>). Default <see langword="true"/>.</summary>
    public bool Minified { get; set; } = true;

    /// <summary>
    /// Add subresource integrity hashes (and <c>crossorigin="anonymous"</c>) for the pinned htmx
    /// version (<see cref="HtmxAssets.Version"/>), so a CDN cannot serve altered files.
    /// <see langword="null"/> (the default) adds them only when <see cref="BaseUrl"/> is the default
    /// jsDelivr URL: a copy you serve yourself may be a different version, which the browser would
    /// refuse to run. Set <see langword="true"/> to add them for your own copy of the same version.
    /// </summary>
    public bool? Integrity { get; set; }

    /// <summary>
    /// Load htmx itself. Set to <see langword="false"/> if your layout or bundler already loads htmx
    /// (before <c>&lt;RzScripts /&gt;</c>).
    /// </summary>
    public bool IncludeHtmx { get; set; } = true;

    /// <summary>The URL of an htmx file under <see cref="BaseUrl"/> (for example <c>htmx</c> or <c>ext/hx-sse</c>).</summary>
    internal string UrlFor(string file)
    {
        var baseUrl = BaseUrl.EndsWith('/') ? BaseUrl : BaseUrl + "/";
        return $"{baseUrl}{file}{(Minified ? ".min.js" : ".js")}";
    }

    /// <summary>Whether integrity hashes apply: <see cref="Integrity"/> when set, otherwise only for the default jsDelivr URL.</summary>
    internal bool UsesIntegrity => Integrity ?? string.Equals(
        BaseUrl.TrimEnd('/'), HtmxAssets.CdnBaseUrl.TrimEnd('/'), StringComparison.OrdinalIgnoreCase);

    /// <summary>The integrity hash for an htmx file, or <see langword="null"/> when integrity does not apply or the file is unknown.</summary>
    internal string? IntegrityFor(string file)
        => UsesIntegrity && HtmxAssets.Integrity.TryGetValue(file + (Minified ? ".min.js" : ".js"), out var hash) ? hash : null;
}
