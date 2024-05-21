using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Rizzy.Antiforgery;
using Rizzy.Configuration;
using Rizzy.Configuration.Htmx;
using Rizzy.Framework.Services;
using System.Text.Json;
using Rizzy.Configuration.Serialization;
using HtmxJsonSerializerContext = Rizzy.Serialization.HtmxJsonSerializerContext;

namespace Rizzy.Components;

/// <summary>
/// This component will render a meta tag with the serialized <see cref="HtmxConfig"/> object,
/// enabling customization of Htmx.
/// </summary>
public class HtmxConfigHeadOutlet : ComponentBase
{
    private string _jsonConfig = string.Empty;

    [Inject] private RzViewContext ViewContext { get; set; } = default!;
    [Inject] private IOptionsSnapshot<HtmxConfig> Options { get; set; } = default!;
    [Inject] private IAntiforgery Antiforgery { get; set; } = default!;
    [Inject] private IOptionsSnapshot<RizzyConfig> RizzyConfig { get; set; } = default!;
    [Inject] private IOptions<HtmxAntiforgeryOptions> AntiforgeryConfig { get; set; } = default!;

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
        var config = ViewContext.Htmx.Configuration;

        if (!string.IsNullOrEmpty(Configuration))
        {
            config = Options.Get(Configuration);
            ViewContext.Htmx.SetConfiguration(Configuration);
        }

        var contextUserConfig = config;

        if (RizzyConfig.Value.AntiforgeryStrategy != AntiforgeryStrategy.None)
        {
            contextUserConfig = contextUserConfig with
            {
                Antiforgery = new HtmxConfig.AntiForgeryConfiguration
                {
                    CookieName = AntiforgeryConfig.Value.CookieName,
                    FormFieldName = AntiforgeryConfig.Value.FormFieldName,
                    HeaderName = AntiforgeryConfig.Value.HeaderName,
                }
            };

            if (RizzyConfig.Value.AntiforgeryStrategy == AntiforgeryStrategy.GenerateTokensPerPage)
            {
                var tokens = Antiforgery.GetAndStoreTokens(HttpContext!);

                contextUserConfig.Antiforgery.RequestToken = tokens.RequestToken!;
            }
        }

        _jsonConfig = JsonSerializer.Serialize(contextUserConfig, HtmxJsonSerializerContext.Default.HtmxConfig);

        return Task.CompletedTask;
    }
}
