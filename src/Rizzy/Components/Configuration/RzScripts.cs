using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Options;
using Rizzy.Htmx;

namespace Rizzy;

/// <summary>
/// Loads htmx, the Rizzy client script and every extension enabled with
/// <see cref="HtmxConfig.UseExtensions"/>, in the order htmx needs. Place it in the root layout's
/// <c>&lt;head&gt;</c>, right after <see cref="RzHeadOutlet"/>:
/// <code>
/// &lt;RzHeadOutlet /&gt;
/// &lt;RzScripts /&gt;
/// </code>
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>htmx comes from <see cref="HtmxScriptOptions.BaseUrl"/> (jsDelivr for the tested version by
/// default) with subresource integrity hashes; see <see cref="HtmxConfig.Scripts"/>. The Rizzy
/// script URL is <c>RizzyConfig.ClientScriptUrl</c>.</item>
/// <item>The Rizzy script loads before the extensions so its diagnostics can report an extension
/// that htmx rejected or that never registered.</item>
/// <item>Every script carries the request's CSP nonce (<see cref="IRizzyNonceProvider"/>), so a
/// nonce-based Content Security Policy works without further changes.</item>
/// </list>
/// </remarks>
public sealed class RzScripts : ComponentBase
{
    [Inject] private IOptionsMonitor<HtmxConfig> Options { get; set; } = default!;
    [Inject] private IRizzyNonceProvider NonceProvider { get; set; } = default!;
    [Inject] private IOptionsMonitor<Rizzy.Configuration.RizzyConfig> RizzyOptions { get; set; } = default!;

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var config = Options.CurrentValue;
        var scripts = config.Scripts;
        var nonce = NonceProvider.GetNonce();
        var sources = new List<(string Src, string? Integrity)>();
        if (scripts.IncludeHtmx)
            sources.Add((scripts.UrlFor("htmx"), scripts.IntegrityFor("htmx")));
        sources.Add((RizzyOptions.CurrentValue.ClientScriptUrl, null));
        foreach (var extension in config.EnabledExtensions)
        {
            if (extension.FileName is { } file)
                sources.Add((scripts.UrlFor("ext/" + file), scripts.IntegrityFor("ext/" + file)));
            else if (extension.ScriptUrl is { } url)
                sources.Add((url, null));
        }

        foreach (var (src, integrity) in sources)
        {
            builder.OpenElement(0, "script");
            builder.AddAttribute(1, "src", src);
            if (integrity is not null)
            {
                builder.AddAttribute(2, "integrity", integrity);
                builder.AddAttribute(3, "crossorigin", "anonymous");
            }
            builder.AddAttribute(4, "nonce", nonce);
            builder.CloseElement();
        }
    }
}
