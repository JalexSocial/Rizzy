using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rizzy.Configuration.Htmx;

namespace Rizzy.Http;

/// <summary>
///  Provides access to Htmx Request and Responses 
/// </summary>
public class HtmxContext
{
    private readonly IOptionsSnapshot<HtmxConfig> _configOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmxContext"/> class.
    /// </summary>
    /// <param name="context"></param>
    public HtmxContext(HttpContext context)
    {
        Request = new HtmxRequest(context);
        Response = new HtmxResponse(context);

        _configOptions = context.RequestServices.GetRequiredService<IOptionsSnapshot<HtmxConfig>>();
        
        Configuration = _configOptions.Value;
    }

    /// <summary>
    /// Gets the HTMX request headers associated with the current request.
    /// </summary>
    public HtmxRequest Request { get; }

    /// <summary>
    /// Allow manipulation of Response headers for the current response
    /// </summary>
    public HtmxResponse Response { get; }

    /// <summary>
    /// Current configuration used with the page
    /// </summary>
    public HtmxConfig Configuration { get; private set; }

    /// <summary>
    /// Sets the HTMX configuration settings for the current application.
    /// </summary>
    /// <param name="name">Optional. The name of the configuration settings to set. If not provided, the default configuration settings will be used.</param>
    public void SetConfiguration(string? name = null)
    {
        Configuration = string.IsNullOrEmpty(name) ?
            _configOptions.Value : _configOptions.Get(name);
    }
}
