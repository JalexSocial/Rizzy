using Microsoft.AspNetCore.Http;

namespace Rizzy.Http;

/// <summary>
///  Provides access to Htmx Request and Responses 
/// </summary>
public class HtmxContext
{
    public HtmxRequest Request { get; }

    public HtmxResponse Response { get; }

    public HtmxContext(HttpContext context)
    {
        Request = new HtmxRequest(context);
        Response = new HtmxResponse(context);
    }
}
