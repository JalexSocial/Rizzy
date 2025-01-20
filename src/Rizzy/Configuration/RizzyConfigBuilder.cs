using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Rizzy.Components;
using Rizzy.Nonce;
using Rizzy.Serialization;
using Rizzy.Utility;
using System;
using Rizzy.Htmx;

namespace Rizzy.Configuration;

/// <summary>
/// Builds and registers the necessary Rizzy services and options.
/// </summary>
public class RizzyConfigBuilder
{
    // A default name for the anti-forgery cookie.
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

        // Register HttpContextAccessor.
        _services.AddHttpContextAccessor();

        // Register the nonce-related services.
        _services.AddRizzyNonceProvider(options =>
        {
	        var sp = _services.BuildServiceProvider();
	        var rizzyConfig = sp.GetRequiredService<IOptions<RizzyConfig>>().Value;
	        options.HMACKey = rizzyConfig.NonceHMACKey;
        });

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

        // Add other necessary services.
        _services.AddSingleton<DataAnnotationsProcessor>();
        _services.AddScoped<IRizzyService, RizzyService>();
        _services.AddScoped<IHtmxSwapService, Rizzy.Components.HtmxSwapService>();
    }
}
