using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Rizzy.Antiforgery;
using Rizzy.Configuration.Serialization;
using Rizzy.Mvc;
using Exception = System.Exception;

namespace Rizzy.Configuration;

/// <summary>
/// This component will render a meta tag with the serialized <see cref="HtmxConfig"/> object,
/// enabling customization of Htmx.
/// </summary>
public class HtmxConfigHeadOutlet : ComponentBase
{
	private string _jsonConfig = string.Empty;

	[Inject] private IOptionsSnapshot<HtmxConfig> Options { get; set; } = default!;
    [Inject] private IAntiforgery Antiforgery { get; set; } = default!;
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
        var config = string.IsNullOrEmpty(Configuration) ?
            Options.Value : Options.Get(Configuration);

        if (AntiforgeryConfig.Value.IncludeAntiForgery)
        {
            var tokens = Antiforgery.GetAndStoreTokens(HttpContext!);

            throw new Exception("Cannot modify singleton config with a scoped antiforgery request token");

            if (config.Antiforgery != null)
                config.Antiforgery.RequestToken = tokens.RequestToken!;
        }

        _jsonConfig = JsonSerializer.Serialize(config, HtmxConfig.JsonTypeInfo);

        return Task.CompletedTask;
    }
}
