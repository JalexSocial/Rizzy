using Microsoft.AspNetCore.Components.Forms.Mapping;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Components.Forms;
using Rizzy.Configuration;
using Rizzy.Htmx;

namespace Rizzy;

/// <summary>
/// This class has extension methods for <see cref="IServiceCollection"/> that enable configuration of Htmx in the application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures Rizzy.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Also registers what Rizzy depends on, so this call alone is enough: htmx support
    /// (<see cref="Htmx.ServiceCollectionExtensions.AddHtmx"/>, which you can still call to configure
    /// <see cref="HtmxConfig"/>) and Razor components (<c>AddRazorComponents()</c>, unless the app
    /// already registered it).
    /// </para>
    /// <para>
    /// Safe to call more than once: services are registered once, and every
    /// <paramref name="configBuilder"/> action is applied in call order. The configuration is
    /// validated when the app starts.
    /// </para>
    /// </remarks>
    /// <param name="services">The service collection.</param>
    /// <param name="configBuilder">An optional action to configure <see cref="RizzyConfig"/>.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddRizzy(this IServiceCollection services, Action<RizzyConfig>? configBuilder = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHtmx();

        // [SupplyParameterFromForm] and Rizzy's form mapping need the Razor component services.
        if (!services.Any(d => d.ServiceType == typeof(IFormValueMapper)))
            services.AddRazorComponents();

        if (!services.Any(d => d.ServiceType == typeof(RizzyServicesMarker)))
        {
            services.AddSingleton<RizzyServicesMarker>();
            services.AddSupplyValueFromFormProvider();
            services.AddTransient<Microsoft.AspNetCore.Hosting.IStartupFilter, RizzyStartupDiagnostics>();
            services.AddOptions<RizzyConfig>()
                .Validate(RizzyConfigValidation.IsValid, RizzyConfigValidation.Message)
                .ValidateOnStart();
        }

        services.TryAddSingleton<DataAnnotationsProcessor>();
        services.TryAddScoped<IRizzyService, RizzyService>();
        services.TryAddScoped<IHtmxSwapService, HtmxSwapService>();
        services.TryAddScoped<IRizzyNonceProvider, RizzyNonceProvider>();

        if (configBuilder is not null)
            services.Configure(configBuilder);

        return services;
    }

    /// <summary>Marks that <see cref="AddRizzy"/> has registered its services.</summary>
    private sealed class RizzyServicesMarker;

    private static class RizzyConfigValidation
    {
        public const string Message =
            "RizzyConfig.RootComponent must be set to HtmxApp<TLayout> (for example HtmxApp<AppLayout>). " +
            "Rizzy renders every view inside it.";

        public static bool IsValid(RizzyConfig config) => config.RootComponent is not null;
    }
}
