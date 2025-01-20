using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Rizzy.Antiforgery;
using Rizzy.Components;
using Rizzy.Nonce;
using Rizzy.Serialization;
using Rizzy.Utility;
using System;

namespace Rizzy.Configuration;

/// <summary>
/// Builds and registers the necessary Rizzy services and options.
/// </summary>
public class RizzyConfigBuilder
{
    // A default name for the anti-forgery cookie.
    private readonly string _defaultCookieName = "HX-XSRF-TOKEN";
    private readonly IServiceCollection _services;
    private readonly IConfiguration? _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="RizzyConfigBuilder"/> class.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configBuilder">
    /// An optional action to configure the <see cref="RizzyConfig"/>.
    /// If not provided, defaults will be applied.
    /// </param>
    /// <param name="configuration">
    /// Optionally, an <see cref="IConfiguration"/> instance to allow reading configuration values.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <c>null</c>.</exception>
    public RizzyConfigBuilder(
        IServiceCollection services,
        Action<RizzyConfig>? configBuilder = null,
        IConfiguration? configuration = null)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _configuration = configuration;

        // Add Htmx anti-forgery support.
        AddHtmxAntiForgery();

        // Register the nonce-related services.
        _services.TryAddSingleton<RizzyNonceGenerator>();
        _services.TryAddScoped<IRizzyNonceProvider, RizzyNonceProvider>();

        // Register HttpContextAccessor.
        _services.AddHttpContextAccessor();

        // Configure the RizzyConfig.
        if (configBuilder != null)
        {
            _services.Configure(configBuilder);
        }
        else
        {
            _services.Configure<RizzyConfig>(cfg =>
            {
                cfg.RootComponent = typeof(EmptyRootComponent);
                cfg.DefaultLayout = null;
            });
        }

        _services.Configure<NonceOptions>(options =>
        {
            var sp = _services.BuildServiceProvider();
            var rizzyConfig = sp.GetRequiredService<IOptions<RizzyConfig>>().Value;
            options.HMACKey = rizzyConfig.NonceHMACKey;
        });

        // Register configuration for Htmx.
        _services.TryAddSingleton<IConfigureOptions<HtmxConfig>, ConfigureHtmxSettings>();
        _services.TryAddSingleton<IConfigureNamedOptions<HtmxConfig>, ConfigureHtmxSettings>();

        // Configure HtmxConfig (an empty configuration delegate here, can be customized later).
        _services.Configure<HtmxConfig>(config =>
        {
            // No-op by default; users can override by calling WithHtmxConfiguration.
        });

        // Add other necessary services.
        _services.AddSingleton<DataAnnotationsProcessor>();
        _services.AddScoped<RzViewContext>();
        _services.AddScoped<IRizzyService, RizzyService>();
        _services.AddScoped<IHtmxSwapService, Rizzy.Components.HtmxSwapService>();
    }

    /// <summary>
    /// Configures the <see cref="HtmxConfig"/> options.
    /// </summary>
    /// <param name="configBuilder">An action to configure the <see cref="HtmxConfig"/> options.</param>
    /// <returns>The current <see cref="RizzyConfigBuilder"/> instance.</returns>
    public RizzyConfigBuilder WithHtmxConfiguration(Action<HtmxConfig> configBuilder)
    {
        _services.Configure(configBuilder);
        return this;
    }

    /// <summary>
    /// Configures a named instance of <see cref="HtmxConfig"/> options.
    /// </summary>
    /// <param name="name">The name for the options instance.</param>
    /// <param name="configBuilder">An action to configure the <see cref="HtmxConfig"/> options.</param>
    /// <returns>The current <see cref="RizzyConfigBuilder"/> instance.</returns>
    public RizzyConfigBuilder WithHtmxConfiguration(string name, Action<HtmxConfig> configBuilder)
    {
        _services.Configure(name, configBuilder);
        return this;
    }

    /// <summary>
    /// Adds and configures Htmx anti-forgery options by reading from the registered <see cref="AntiforgeryOptions"/>.
    /// </summary>
    private void AddHtmxAntiForgery()
    {
        // Resolve the current AntiforgeryOptions instance.
        // Note: We build a temporary ServiceProvider for resolving the options.
        using (var sp = _services.BuildServiceProvider())
        {
            var antiforgeryOptions = sp.GetRequiredService<IOptions<AntiforgeryOptions>>();
            _services.Configure<HtmxAntiforgeryOptions>(opt =>
            {
                opt.FormFieldName = antiforgeryOptions.Value.FormFieldName;
                opt.HeaderName = antiforgeryOptions.Value.HeaderName;
                opt.CookieName = _defaultCookieName;
            });
        }
    }

    /// <summary>
    /// Class responsible for configuring <see cref="HtmxConfig"/> options.
    /// </summary>
    public class ConfigureHtmxSettings : IConfigureOptions<HtmxConfig>, IConfigureNamedOptions<HtmxConfig>
    {
        private readonly HtmxAntiforgeryOptions _antiforgeryOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureHtmxSettings"/> class.
        /// </summary>
        /// <param name="antiforgeryOptions">The antiforgery options.</param>
        public ConfigureHtmxSettings(IOptions<HtmxAntiforgeryOptions> antiforgeryOptions)
        {
            _antiforgeryOptions = antiforgeryOptions.Value;
        }

        /// <summary>
        /// Configures a named instance of <see cref="HtmxConfig"/> options.
        /// </summary>
        /// <param name="name">The name of the options instance.</param>
        /// <param name="options">The <see cref="HtmxConfig"/> options to configure.</param>
        public void Configure(string? name, HtmxConfig options)
        {
            Configure(options);
        }

        /// <summary>
        /// Configures the default <see cref="HtmxConfig"/> options.
        /// </summary>
        /// <param name="options">The <see cref="HtmxConfig"/> options to configure.</param>
        public void Configure(HtmxConfig options)
        {
        }
    }
}
