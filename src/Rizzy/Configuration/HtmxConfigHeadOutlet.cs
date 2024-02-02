﻿using System.Text.Json;
using Microsoft.AspNetCore.Components;
using HtmxConfigJsonSerializerContext = Htmxor.Configuration.Serialization.HtmxConfigJsonSerializerContext;

namespace Rizzy.Configuration;

/// <summary>
/// This component will render a meta tag with the serialized <see cref="HtmxConfig"/> object,
/// enabling customization of Htmx.
/// </summary>
/// <remarks>
/// Configure the <see cref="HtmxConfig"/> via the 
/// <see cref="HtmxorApplicationBuilderExtensions.AddHtmx(Microsoft.Extensions.Hosting.IHostApplicationBuilder, Action{Htmxor.Configuration.HtmxConfig}?)"/> 
/// method.
/// </remarks>
public class HtmxConfigHeadOutlet : IComponent
{
    [Inject] private HtmxConfig Config { get; set; } = default!;

    /// <inheritdoc/>
    public void Attach(RenderHandle renderHandle)
    {
        var json = JsonSerializer.Serialize(Config, HtmxConfigJsonSerializerContext.Default.HtmxConfig);
        renderHandle.Render(builder =>
        {
            builder.AddMarkupContent(0, @$"<meta name=""htmx-config"" content='{json}'>");
        });
    }

    /// <inheritdoc/>
    public Task SetParametersAsync(ParameterView parameters) => Task.CompletedTask;
}
