using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Rizzy.Htmx;

/// <summary>
/// Provides extension methods to add HTMX services and configurations to the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
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

        // Configure the default HtmxConfig.
        services.Configure<HtmxConfig>(configure ?? (options => { }));

        return services;
    }

    /// <summary>
    /// Adds and configures Htmx anti-forgery options by reading from the registered <see cref="AntiforgeryOptions"/>.
    /// </summary>
    private static void AddHtmxAntiForgery(IServiceCollection services)
    {
        services.AddAntiforgery();

        services.AddOptions<HtmxAntiforgeryOptions>()
            .Configure<IOptions<AntiforgeryOptions>>(
                (htmxOpt, antiOpt) =>
                {
                    htmxOpt.FormFieldName = antiOpt.Value.FormFieldName;
                    htmxOpt.HeaderName    = antiOpt.Value.HeaderName;
                    htmxOpt.CookieName    = Constants.AntiforgeryCookieName;
                });

        services.AddAntiforgeryValidation();
    }

    /// <summary>
    /// Add antiforgery validation services (uses reflection)
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    private static IServiceCollection AddAntiforgeryValidation(
        this IServiceCollection services)
    {
        var types = Assembly.Load("Microsoft.AspNetCore.Mvc.ViewFeatures")
            .GetTypes();
        var autoType = types.First(t => t.Name == "AutoValidateAntiforgeryTokenAuthorizationFilter"); // necessary for the AutoValidateAntiforgeryTokenAttribute
        var defaultType = types.First(t => t.Name == "ValidateAntiforgeryTokenAuthorizationFilter"); // necessary for the ValidateAntiforgeryTokenAttribute
        services.TryAddSingleton(autoType);
        services.TryAddSingleton(defaultType);

        return services;
    }

    /// <summary>
    /// Class responsible for configuring <see cref="HtmxConfig"/> options.
    /// </summary>
    public class ConfigureHtmxSettings : IConfigureOptions<HtmxConfig>
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
        /// Configures the default <see cref="HtmxConfig"/> options.
        /// </summary>
        /// <param name="options">The <see cref="HtmxConfig"/> options to configure.</param>
        public void Configure(HtmxConfig options)
        {
        }
    }
}
