using Microsoft.AspNetCore.Http;

namespace Rizzy.Htmx;

/// <summary>
///  Provides access to Htmx Request and Responses 
/// </summary>
public class HtmxContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HtmxContext"/> class.
    /// </summary>
    /// <param name="context"></param>
    public HtmxContext(HttpContext context)
    {
        Request = new HtmxRequest(context);
        Response = new HtmxResponse(context);
    }

    /// <summary>
    /// Gets the HTMX request headers associated with the current request.
    /// </summary>
    public HtmxRequest Request { get; }

    /// <summary>
    /// Allow manipulation of Response headers for the current response
    /// </summary>
    public HtmxResponse Response { get; }
}
