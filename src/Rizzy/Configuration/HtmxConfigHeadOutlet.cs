using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Rizzy.Configuration.Serialization;

namespace Rizzy.Configuration;

/// <summary>
/// This component will render a meta tag with the serialized <see cref="HtmxConfig"/> object,
/// enabling customization of Htmx.
/// </summary>
public class HtmxConfigHeadOutlet : IComponent
{
	private string _jsonConfig = string.Empty;

	[Inject] private IOptionsSnapshot<HtmxConfig> Options { get; set; } = default!;

	/// <summary>
	/// Specify a named configuration to use a non-default configuration
	/// </summary>
	[Parameter] public string? Configuration { get; set; } = default!;

	/// <inheritdoc/>
	public void Attach(RenderHandle renderHandle)
	{
		renderHandle.Render(builder =>
		{
			builder.AddMarkupContent(0, @$"<meta name=""htmx-config"" content='{_jsonConfig}'>");
		});
	}

	public Task SetParametersAsync(ParameterView parameters)
	{
		Configuration = parameters.GetValueOrDefault<string?>("Configuration");

		var config = string.IsNullOrEmpty(Configuration) ?
			Options.Value : Options.Get(Configuration);

		_jsonConfig = JsonSerializer.Serialize(config, HtmxConfig.JsonTypeInfo);

		return Task.CompletedTask;
	}
}
