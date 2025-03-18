using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Rizzy;
using Rizzy.Htmx;

namespace Rizzy;

/// <summary>
/// Middleware that checks if the current request is an HTMX request and, if so, adds a nonce header to the request.
/// </summary>
public class RizzyMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="RizzyMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    public RizzyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Processes the HTTP request. If the request is determined to be an HTMX request, it adds a nonce header to the request.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="nonceProvider">The service that provides the nonce value.</param>
    /// <returns>A task that represents the completion of the middleware execution.</returns>
    public async Task InvokeAsync(HttpContext context, IRizzyNonceProvider nonceProvider)
    {
        // Check if the current request is an HTMX request.
        if (context.Request.IsHtmx())
        {
            // Get the nonce value from the provider.
            var nonce = nonceProvider.GetNonce(); 
            
            // Add the nonce to the request headers.
            context.Response.Headers.TryAdd(Constants.NonceResponseHeader, nonce);
        }

        await _next(context);
    }
}