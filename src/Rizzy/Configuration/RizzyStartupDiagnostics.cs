using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Rizzy.Configuration;

/// <summary>
/// Logs configuration problems that would otherwise fail silently (RIZZY-026).
/// </summary>
internal sealed partial class RizzyStartupDiagnostics(IOptions<RizzyConfig> config, ILogger<RizzyStartupDiagnostics> logger) : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        if (config.Value.RootComponent == typeof(HtmxApp<EmptyLayout>))
            LogDefaultRootComponent(logger);

        return next;
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message =
        "RizzyConfig.RootComponent is the default HtmxApp<EmptyLayout>, which renders no <head> or <HeadOutlet />. " +
        "The htmx-config meta tag (htmx settings and the antiforgery token) will not be rendered, so unsafe htmx requests " +
        "will fail antiforgery validation. Set RootComponent to HtmxApp<YourRootLayout>, where the layout renders <HeadOutlet />.")]
    private static partial void LogDefaultRootComponent(ILogger logger);
}
