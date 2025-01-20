using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Rizzy.Htmx;

/// <summary>
/// Provides extension methods to add HTMX services and configurations to the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
	private static readonly string _defaultCookieName = "HX-XSRF-TOKEN";

    /// <summary>
    /// Adds the Rizzy.Htmx services to the service collection with an optional default configuration.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configure">An optional action to configure the default HtmxConfig.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddHtmx(this IServiceCollection services, Action<HtmxConfig>? configure = null)
    {
        // Ensure IHttpContextAccessor is registered.
        services.AddHttpContextAccessor();

        // Add Htmx anti-forgery support.
        AddHtmxAntiForgery(services);

        // Register configuration for Htmx.
        services.TryAddSingleton<IConfigureOptions<HtmxConfig>, ConfigureHtmxSettings>();
        services.TryAddSingleton<IConfigureNamedOptions<HtmxConfig>, ConfigureHtmxSettings>();

        // Configure the default HtmxConfig.
        services.Configure<HtmxConfig>(configure ?? (options => { }));

        return services;
    }

    /// <summary>
    /// Adds a named Rizzy.Htmx configuration to the service collection.
    /// This allows multiple named instances of HtmxConfig.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="name">The name identifier for the HtmxConfig instance.</param>
    /// <param name="configure">An action to configure the named HtmxConfig.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddNamedHtmxConfiguration(this IServiceCollection services, string name, Action<HtmxConfig> configure)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Configuration name must be a non-empty string.", nameof(name));

        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        // Configure the named HtmxConfig.
        services.Configure<HtmxConfig>(name, configure);

        return services;
    }

    /// <summary>
    /// Adds and configures Htmx anti-forgery options by reading from the registered <see cref="AntiforgeryOptions"/>.
    /// </summary>
    private static void AddHtmxAntiForgery(IServiceCollection services)
    {
        // Resolve the current AntiforgeryOptions instance.
        // Note: We build a temporary ServiceProvider for resolving the options.
        using var sp = services.BuildServiceProvider();

        var antiforgeryOptions = sp.GetRequiredService<IOptions<AntiforgeryOptions>>();
        services.Configure<HtmxAntiforgeryOptions>(opt =>
        {
            opt.FormFieldName = antiforgeryOptions.Value.FormFieldName;
            opt.HeaderName = antiforgeryOptions.Value.HeaderName;
            opt.CookieName = _defaultCookieName;
        });
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
