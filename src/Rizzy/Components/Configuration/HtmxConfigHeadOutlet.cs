using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Rizzy.Configuration;
using Rizzy.Htmx;
using System.Web;

namespace Rizzy;

/// <summary>
/// This component will render a meta tag with the serialized <see cref="HtmxConfig"/> object,
/// enabling customization of Htmx.
/// </summary>
public class HtmxConfigHeadOutlet : ComponentBase
{
    private static readonly string RizzyVersion =
        typeof(HtmxConfigHeadOutlet).Assembly
            .GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)
            .OfType<System.Reflection.AssemblyInformationalVersionAttribute>()
            .FirstOrDefault()?.InformationalVersion.Split('+')[0]
        ?? typeof(HtmxConfigHeadOutlet).Assembly.GetName().Version?.ToString()
        ?? "unknown";

    private string _jsonConfig = string.Empty;
    [Inject] private IRizzyNonceProvider? NonceProvider { get; set; }
    [Inject] private IOptionsSnapshot<HtmxConfig> Options { get; set; } = default!;
    [Inject] private IAntiforgery Antiforgery { get; set; } = default!;
    [Inject] private IOptionsSnapshot<RizzyConfig> RizzyConfig { get; set; } = default!;
    [Inject] private IOptions<HtmxAntiforgeryOptions> AntiforgeryConfig { get; set; } = default!;

    [CascadingParameter]
    public HttpContext? HttpContext { get; set; }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AddMarkupContent(0, @$"<meta name=""htmx-config"" content='{_jsonConfig}'>");
    }

    protected override Task OnParametersSetAsync()
    {
        // Work on a copy: the options instance is shared by every request.
        var config = Options.Value with { };

        // Keep the htmx.config.extensions allow-list in step with the enabled extensions, so htmx
        // never silently rejects Rizzy's own extensions or one enabled with UseExtensions.
        config.Extensions = config.ResolveExtensionAllowList(HtmxExtension.Rizzy.Select(e => e.Name));
        config.ExpectedExtensions = config.EnabledExtensions.Count > 0
            ? config.EnabledExtensions.Select(e => e.Name).ToArray()
            : null;

        if (RizzyConfig.Value.AntiforgeryStrategy == AntiforgeryStrategy.GenerateTokensPerPage)
        {
            config = config with
            {
                Antiforgery = new HtmxConfig.AntiForgeryConfiguration
                {
                    FormFieldName = AntiforgeryConfig.Value.FormFieldName,
                    HeaderName = AntiforgeryConfig.Value.HeaderName,
                }
            };

            var tokens = Antiforgery.GetAndStoreTokens(HttpContext!);

            config.Antiforgery.RequestToken = HttpUtility.HtmlAttributeEncode(tokens.RequestToken)!;
        }

        // The page nonce is not written into the meta tag: the client reads it from the nonce
        // property of the page's <script nonce> elements, which CSS selectors cannot read.
        if (config.GenerateScriptNonce)
            config.InlineScriptNonce = NonceProvider?.GetNonce() ?? string.Empty;

        config.RizzyVersion = RizzyVersion;

        _jsonConfig = config.Serialize();

        return Task.CompletedTask;
    }
}
