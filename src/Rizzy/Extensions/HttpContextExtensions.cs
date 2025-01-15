using Microsoft.AspNetCore.Http;
using Rizzy.Http;

namespace Rizzy;

public static class HttpContextExtensions
{
	private const string HtmxRequestKey = "Rizzy.Http:HtmxRequest";

    /// <summary>
    /// Extension method for HttpRequest that creates (or returns a cached) HtmxRequest.
    /// </summary>
    /// <param name="request">The current HttpRequest.</param>
    /// <returns>An instance of HtmxResponse.</returns>
    public static HtmxRequest Htmx(this HttpRequest request)
    {
	    ArgumentNullException.ThrowIfNull(request);

	    // Check if the HtmxRequest is already created and stored in HttpContext.Items.
	    if (request.HttpContext.Items.TryGetValue(HtmxRequestKey, out var existing) && existing is HtmxRequest htmxRequest)
	    {
		    return htmxRequest;
	    }

	    // Create new HtmxResponse and store it in HttpContext.Items.
	    htmxRequest = new HtmxRequest(request.HttpContext);
	    request.HttpContext.Items[HtmxRequestKey] = htmxRequest;

	    return htmxRequest;
    }

    /// <summary>
    /// Extension method for HttpResponse that creates a new HtmxResponse and invokes the provided action.
    /// </summary>
    /// <param name="response">The current HttpResponse.</param>
    /// <param name="action">An action that receives an HtmxResponse.</param>
    public static void Htmx(this HttpResponse response, Action<HtmxResponse> action)
    {
	    ArgumentNullException.ThrowIfNull(response);
	    ArgumentNullException.ThrowIfNull(action);

	    // Create a new HtmxResponse.
	    var htmxResponse = new HtmxResponse(response.HttpContext);

	    // Invoke the action delegate, passing the newly created HtmxResponse.
	    action(htmxResponse);
    }

    /// <summary>
    /// Extension method for HttpResponse that creates a new HtmxResponse and returns it.
    /// </summary>
    /// <param name="response">The current HttpResponse.</param>
    /// <returns>A new instance of HtmxResponse.</returns>
    public static HtmxResponse Htmx(this HttpResponse response)
    {
	    ArgumentNullException.ThrowIfNull(response);

	    return new HtmxResponse(response.HttpContext);
    }
}