using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Rizzy.Configuration;
using System.Text.Json;
using Rizzy.Htmx;
using Rizzy.Nonce;

namespace Rizzy.Components;

/// <summary>
/// This component will render a meta tag with the serialized <see cref="HtmxConfig"/> object,
/// enabling customization of Htmx.
/// </summary>
public class HtmxConfigHeadOutlet : ComponentBase
{
    private string _jsonConfig = string.Empty;

    [Inject] private IOptionsSnapshot<HtmxConfig> Options { get; set; } = default!;
    [Inject] private IAntiforgery Antiforgery { get; set; } = default!;
    [Inject] private IOptionsSnapshot<RizzyConfig> RizzyConfig { get; set; } = default!;
    [Inject] private IOptions<HtmxAntiforgeryOptions> AntiforgeryConfig { get; set; } = default!;
    [Inject] private IRizzyNonceProvider NonceProvider { get; set; } = default!;

    [CascadingParameter]
    public HttpContext? HttpContext { get; set; }

    /// <summary>
    /// Specify a named configuration to use a non-default configuration
    /// </summary>
    [Parameter] public string? Configuration { get; set; } = default!;

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AddMarkupContent(0, @$"<meta name=""htmx-config"" content='{_jsonConfig}'>");
    }

    protected override Task OnParametersSetAsync()
    {
        var config = string.IsNullOrEmpty(Configuration) ? Options.Value : Options.Get(Configuration);

        if (RizzyConfig.Value.AntiforgeryStrategy == AntiforgeryStrategy.GenerateTokensPerPage)
        {
            config = config with
            {
                Antiforgery = new HtmxConfig.AntiForgeryConfiguration
                {
                    CookieName = AntiforgeryConfig.Value.CookieName,
                    FormFieldName = AntiforgeryConfig.Value.FormFieldName,
                    HeaderName = AntiforgeryConfig.Value.HeaderName,
                }
            };

            var tokens = Antiforgery.GetAndStoreTokens(HttpContext!);

            config.Antiforgery.RequestToken = tokens.RequestToken!;
        }

        if (config.GenerateScriptNonce)
            config.InlineScriptNonce = NonceProvider.GetNonceFor(NonceType.Script);

        if (config.GenerateStyleNonce)
            config.InlineStyleNonce = NonceProvider.GetNonceFor(NonceType.Style);

        _jsonConfig = config.Serialize();

        return Task.CompletedTask;
    }
}
