﻿using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Rizzy.Antiforgery;
using Rizzy.Components;
using Rizzy.Components.Form;
using Rizzy.Components.Form.Helpers;
using Rizzy.Nonce;

namespace Rizzy.Configuration;

public class RizzyConfigBuilder
{
    private readonly string _defaultCookieName = "HX-XSRF-TOKEN";
    private readonly IHostApplicationBuilder _builder;

    /// <summary>
    /// Configures HtmxConfig options to include the antiforgery configuration
    /// </summary>
    public class ConfigureHtmxSettings : IConfigureOptions<HtmxConfig>, IConfigureNamedOptions<HtmxConfig>
    {
        private HtmxAntiforgeryOptions _antiforgeryOptions = default!;

        public ConfigureHtmxSettings(IOptions<HtmxAntiforgeryOptions> antiforgeryOptions)
        {
            _antiforgeryOptions = antiforgeryOptions.Value;
        }

        public void Configure(string? name, HtmxConfig options)
        {
            // Do nothing for now
        }

        public void Configure(HtmxConfig options) => Configure(Options.DefaultName, options);
    }

    /// <summary>
    /// Sets up default services for Rizzy
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configBuilder"></param>
    public RizzyConfigBuilder(IHostApplicationBuilder builder, Action<RizzyConfig>? configBuilder)
    {
        _builder = builder;

        AddHtmxAntiForgery();

        // Add nonce generation services
        _builder.Services.TryAddSingleton<RizzyNonceGenerator>();

        // Register the default RizzyNonceProvider only if IRizzyNonceProvider hasn't been registered
        _builder.Services.TryAddScoped<IRizzyNonceProvider, RizzyNonceProvider>();

        _builder.Services.AddHttpContextAccessor();

        if (configBuilder != null)
        {
            _builder.Services.Configure<RizzyConfig>(configBuilder);
        }
        else
        {
            _builder.Services.Configure<RizzyConfig>(cfg =>
            {
                cfg.RootComponent = typeof(EmptyRootComponent);
                cfg.DefaultLayout = null;
            });
        }

        // Make sure all HtmxConfig instances get properly configured
        _builder.Services.AddSingleton<IConfigureOptions<HtmxConfig>, ConfigureHtmxSettings>();
        _builder.Services.AddSingleton<IConfigureNamedOptions<HtmxConfig>, ConfigureHtmxSettings>();

        // Configure a default htmx configuration
        _builder.Services.Configure<HtmxConfig>(config => { });

        _builder.Services.AddSingleton<DataAnnotationsProcessor>();

        // Add additional scoped services
        _builder.Services.AddScoped<RzViewContext>();
        _builder.Services.AddScoped<IRizzyService, RizzyService>();
        _builder.Services.AddScoped<IHtmxSwapService, HtmxSwapService>();
    }

    /// <summary>
    /// Adds a default configuration for use inside an HtmxConfigHeadOutlet
    /// </summary>
    /// <param name="configBuilder"></param>
    /// <returns></returns>
    public RizzyConfigBuilder WithHtmxConfiguration(Action<HtmxConfig> configBuilder)
    {
        _builder.Services.Configure<HtmxConfig>(configBuilder);

        return this;
    }

    /// <summary>
    /// Adds a named configuration for use inside an HtmxConfigHeadOutlet
    /// </summary>
    /// <param name="name"></param>
    /// <param name="configBuilder"></param>
    /// <returns></returns>
    public RizzyConfigBuilder WithHtmxConfiguration(string name, Action<HtmxConfig> configBuilder)
    {
        _builder.Services.Configure<HtmxConfig>(name, configBuilder);

        return this;
    }

    private void AddHtmxAntiForgery()
    {
        var provider = _builder.Services.BuildServiceProvider();
        var antiforgeryOptions = provider.GetRequiredService<IOptions<AntiforgeryOptions>>();

        _builder.Services.Configure<HtmxAntiforgeryOptions>(opt =>
        {
            // Default to true, can be configured again after adding htmx if necessary
            opt.FormFieldName = antiforgeryOptions.Value.FormFieldName;
            opt.HeaderName = antiforgeryOptions.Value.HeaderName;
            opt.CookieName = _defaultCookieName;
        });
    }
}