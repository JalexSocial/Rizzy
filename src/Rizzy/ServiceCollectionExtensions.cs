using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rizzy.Configuration;

namespace Rizzy;

/// <summary>
/// This class has extension methods for <see cref="IServiceCollection"/> that enable configuration of Htmx in the application.
/// </summary>
public static class ServiceCollectionExtensionss
{
    /// <summary>
    /// Add and configure Htmx.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configBuilder"></param>
    public static RizzyConfigBuilder AddRizzy(this IServiceCollection services, Action<RizzyConfig>? configBuilder = null) =>
        new (services, configBuilder);
}
