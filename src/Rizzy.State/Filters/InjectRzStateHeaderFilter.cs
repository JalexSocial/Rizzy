using Microsoft.AspNetCore.Mvc.Filters;
using Rizzy.Htmx; // For HtmxRequestHeaderNames

namespace Rizzy.State.Filters;

/// <summary>
/// Ensures the RZ-State header is added to htmx responses if a new state token
/// has been prepared by a preceding filter (e.g., RizzyPageStateFilter).
/// </summary>
public class InjectRzStateHeaderFilter : IResultFilter
{
	/// <inheritdoc />
	public void OnResultExecuted(ResultExecutedContext context) { } // No action needed after result executed

	/// <inheritdoc />
	public void OnResultExecuting(ResultExecutingContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		// Only act on htmx requests
		if (context.HttpContext.Request.Headers.ContainsKey(HtmxRequestHeaderNames.HtmxRequest))
		{
			if (context.HttpContext.Items.TryGetValue(RizzyStateConstants.HttpContextItems.NewStateForHeader, out var tokenObject) &&
			    tokenObject is string newToken &&
			    !string.IsNullOrEmpty(newToken))
			{
				context.HttpContext.Response.Headers[RizzyStateConstants.HtmxResponseHeaders.RZState] = newToken;
			}
		}
	}
}