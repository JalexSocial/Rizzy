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
	private readonly HttpContext _httpContext;
	private readonly IOptionsSnapshot<HtmxConfig> _configOptions;

	public HtmxContext(HttpContext context)
	{
		Request = new HtmxRequest(context);
		Response = new HtmxResponse(context);

		_httpContext = context;
		_configOptions = context.RequestServices.GetRequiredService<IOptionsSnapshot<HtmxConfig>>();

		Configuration = _configOptions.Value;
	}

    public HtmxRequest Request { get; }

    public HtmxResponse Response { get; }

    public HtmxConfig Configuration { get; private set; }

    public void SetConfiguration(string? name = null)
    {
		Configuration = string.IsNullOrEmpty(name) ?
		    _configOptions.Value : _configOptions.Get(name);
    }
}
