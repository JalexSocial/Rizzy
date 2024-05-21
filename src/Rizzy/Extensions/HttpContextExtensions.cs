using Microsoft.AspNetCore.Http;
using Rizzy.Http;

namespace Rizzy.Extensions;

public static class HttpContextExtensions
{
    public static HtmxContext GetHtmxContext(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        return new HtmxContext(httpContext);
    }
}