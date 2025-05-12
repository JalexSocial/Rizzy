using Microsoft.AspNetCore.DataProtection; // Required for AddDataProtection
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rizzy.State.Filters;
using Rizzy.State.Serialization;
using Rizzy.State.StateHandling;

namespace Rizzy.State.Configuration; // Or just Rizzy.State if preferred

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to configure Rizzy.State services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the core services required for Rizzy.State functionality, including state protection
    /// and view model serialization.
    /// </summary>
    /// <remarks>
    /// This method should be called in your application's service configuration (e.g., Program.cs).
    /// It registers:
    /// <list type="bullet">
    ///   <item><description><see cref="IRizzyViewModelSerializer"/> and its default implementation <see cref="RizzyViewModelSerializer"/> as singletons.</description></item>
    ///   <item><description><see cref="IRizzyStateProtector"/> and its default implementation <see cref="RizzyStateProtector"/> as singletons.</description></item>
    /// </list>
    /// Ensure that ASP.NET Core Data Protection is also configured (e.g., via <c>builder.Services.AddDataProtection().PersistKeysToXxx();</c>).
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is null.</exception>
    public static IServiceCollection AddRizzyStateCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Ensure Data Protection is available (consumers should ideally call AddDataProtection themselves with persistence config)
        // Adding a basic AddDataProtection() here ensures it's present if not already added,
        // but consumers should configure key persistence for production environments.
        services.AddDataProtection(); 

        services.TryAddSingleton<IRizzyViewModelSerializer, RizzyViewModelSerializer>();
        services.TryAddSingleton<IRizzyStateProtector, RizzyStateProtector>();
        
        return services;
    }

    /// <summary>
    /// Adds the necessary MVC filters for Rizzy.State to manage state tokens during request processing.
    /// This method should be called if you are using Rizzy.State with ASP.NET Core MVC Controllers or Razor Pages
    /// that return Blazor components via Rizzy's View/PartialView results.
    /// </summary>
    /// <remarks>
    /// This registers:
    /// <list type="bullet">
    ///   <item><description><see cref="RizzyPageStateFilter"/> as a scoped service and adds it to MVC's global filters.</description></item>
    ///   <item><description><see cref="InjectRzStateHeaderFilter"/> as a scoped service and adds it to MVC's global filters.</description></item>
    /// </list>
    /// Requires MVC services to be configured (e.g., via <c>services.AddControllersWithViews()</c> or <c>services.AddRazorPages()</c>).
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is null.</exception>
    public static IServiceCollection AddRizzyStateMvcFilters(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddScoped<RizzyPageStateFilter>();
        services.TryAddScoped<InjectRzStateHeaderFilter>();

        // Configure MvcOptions to add the filters.
        // This ensures they are added regardless of whether AddControllers, AddRazorPages,
        // or AddMvcCore was called before or after this method.
        services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
        {
            // Add filters with a specific order if necessary, or rely on order of addition.
            // RizzyPageStateFilter should generally run before InjectRzStateHeaderFilter
            // in the Result Filter pipeline stage to ensure 'RZ-State-New' is available.
            // Adding them in this order here should achieve that for the same filter stage.
            options.Filters.Add<RizzyPageStateFilter>();
            options.Filters.Add<InjectRzStateHeaderFilter>();
        });

        return services;
    }

    /// <summary>
    /// A convenience method that registers all core Rizzy.State services and MVC filters.
    /// Calls <see cref="AddRizzyStateCore"/> and <see cref="AddRizzyStateMvcFilters"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddRizzyState(this IServiceCollection services)
    {
        services.AddRizzyStateCore();
        services.AddRizzyStateMvcFilters();
        return services;
    }
}