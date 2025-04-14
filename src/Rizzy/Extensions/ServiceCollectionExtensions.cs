using Microsoft.AspNetCore.Components.Forms.Mapping;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rizzy.Configuration;
using Rizzy.Htmx;

namespace Rizzy;

/// <summary>
/// This class has extension methods for <see cref="IServiceCollection"/> that enable configuration of Htmx in the application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add and configure Rizzy.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configBuilder"></param>
    public static IServiceCollection AddRizzy(this IServiceCollection services, Action<RizzyConfig>? configBuilder = null)
    {

        services = services ?? throw new ArgumentNullException(nameof(services));

        // Register HttpContextAccessor.
        services.AddHttpContextAccessor();
        services.AddSupplyValueFromFormProvider();
        services.Configure(configBuilder ?? (cfg => { }));

        // Add other necessary services.
        services.AddSingleton<DataAnnotationsProcessor>();
        services.AddScoped<IRizzyService, RizzyService>();
        services.AddScoped<IHtmxSwapService, HtmxSwapService>();
        services.TryAddScoped<IRizzyNonceProvider, RizzyNonceProvider>();

        return services;
    }
}
