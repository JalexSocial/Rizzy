using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Rizzy.Htmx;

/// <summary>
/// Provides extension methods to add htmx services and configuration to the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Rizzy.Htmx services to the service collection with an optional configuration.
    /// </summary>
    /// <remarks>
    /// Safe to call more than once (including indirectly through <c>AddRizzy</c>): services are
    /// registered once and every <paramref name="configure"/> action is applied in call order.
    /// <para>
    /// To use <c>[ValidateAntiForgeryToken]</c>/<c>[AutoValidateAntiforgeryToken]</c> on MVC
    /// controllers, register MVC with <c>AddControllersWithViews()</c> (or
    /// <c>AddMvcCore().AddViews()</c>), which provides the antiforgery filters those attributes use.
    /// </para>
    /// </remarks>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configure">An optional action to configure <see cref="HtmxConfig"/>.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddHtmx(this IServiceCollection services, Action<HtmxConfig>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (!services.Any(d => d.ServiceType == typeof(HtmxServicesMarker)))
        {
            services.AddSingleton<HtmxServicesMarker>();
            services.AddHttpContextAccessor();
            services.AddAntiforgery();
            services.AddOptions<HtmxConfig>();
            services.AddOptions<HtmxAntiforgeryOptions>()
                .Configure<IOptions<AntiforgeryOptions>>((htmx, antiforgery) =>
                {
                    htmx.FormFieldName = antiforgery.Value.FormFieldName;
                    htmx.HeaderName = antiforgery.Value.HeaderName;
                });
            services.AddTransient<IStartupFilter, HxAttributesStartupFilter>();
        }

        if (configure is not null)
            services.Configure(configure);

        return services;
    }

    /// <summary>Marks that <see cref="AddHtmx"/> has registered its services.</summary>
    private sealed class HtmxServicesMarker;

    /// <summary>
    /// Copies <see cref="HtmxConfig.MetaCharacter"/> to <see cref="HxAttributes.DefaultMetaCharacter"/>
    /// when the app starts, so attribute names built with <see cref="Hx"/> match what htmx looks up.
    /// </summary>
    private sealed class HxAttributesStartupFilter(IOptions<HtmxConfig> options) : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            // Only when configured, so another host starting in the same process (tests) does
            // not reset a value an app set.
            if (!string.IsNullOrEmpty(options.Value.MetaCharacter))
                HxAttributes.DefaultMetaCharacter = options.Value.MetaCharacter;
            return next;
        }
    }
}
