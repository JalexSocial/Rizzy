using Microsoft.AspNetCore.Mvc.Filters;
using Rizzy.Configuration.Htmx.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rizzy.Http.Attributes;

/// <summary>
/// An attribute to apply HTMX response headers to an action result. This enables server-side control
/// of client-side HTMX behaviors such as redirection, refreshing, and element updates.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class HtmxResponseAttribute : Attribute, IResultFilter
{
	/// <summary>
	/// Specifies a client-side redirect to a new location without a full page reload.
	/// </summary>
	public string? Location { get; set; }

	/// <summary>
	/// Pushes a new URL onto the browser's history stack.
	/// </summary>
	public string? PushUrl { get; set; }

	/// <summary>
	/// Prevents the browser's history from being updated.
	/// </summary>
	public bool? PreventBrowserHistoryUpdate { get; set; }

	/// <summary>
	/// Prevents the browser's current URL from being updated.
	/// </summary>
	public bool? PreventBrowserCurrentUrlUpdate { get; set; }

	/// <summary>
	/// Initiates a client-side redirect to a specified URL.
	/// </summary>
	public string? Redirect { get; set; }

	/// <summary>
	/// Triggers a client-side full page refresh.
	/// </summary>
	public bool? Refresh { get; set; }

	/// <summary>
	/// Replaces the current URL in the browser's location bar.
	/// </summary>
	public string? ReplaceUrl { get; set; }

	/// <summary>
	/// Specifies how the response will be swapped in the client-side, using HTMX swap strategies.
	/// </summary>
	public SwapStyle? Reswap { get; set; }

	/// <summary>
	/// Updates the target of the content update to a different element on the page using a CSS selector.
	/// </summary>
	public string? Retarget { get; set; }

	/// <summary>
	/// Sets return status code to stop an ongoing polling request
	/// </summary>
	public bool StopPolling { get; set; }

	/// <summary>
	/// Chooses which part of the response is used for the swap, using a CSS selector.
	/// </summary>
	public string? Reselect { get; set; }

	/// <summary>
	/// Called before the action result is executed. This method applies the HTMX configurations
	/// specified by the attribute properties to the response headers.
	/// </summary>
	/// <param name="context">The context in which the result is being executed.</param>
	/// <exception cref="ArgumentNullException">Thrown if the context is null.</exception>
	public void OnResultExecuting(ResultExecutingContext context)
	{
		if (context == null) throw new ArgumentNullException(nameof(context));

		var htmxResponse = new HtmxResponse(context.HttpContext);

		// Dynamically apply the HTMX response configurations based on set properties
		if (!string.IsNullOrEmpty(Location)) htmxResponse.Location(Location);
		if (!string.IsNullOrEmpty(PushUrl)) htmxResponse.PushUrl(PushUrl);
		if (PreventBrowserHistoryUpdate == true) htmxResponse.PreventBrowserHistoryUpdate();
		if (PreventBrowserCurrentUrlUpdate == true) htmxResponse.PreventBrowserCurrentUrlUpdate();
		if (!string.IsNullOrEmpty(Redirect)) htmxResponse.Redirect(Redirect);
		if (Refresh == true) htmxResponse.Refresh();
		if (!string.IsNullOrEmpty(ReplaceUrl)) htmxResponse.ReplaceUrl(ReplaceUrl);
		if (Reswap != null) htmxResponse.Reswap((SwapStyle)Reswap, null);
		if (!string.IsNullOrEmpty(Retarget)) htmxResponse.Retarget(Retarget);
		if (!string.IsNullOrEmpty(Reselect)) htmxResponse.Reselect(Reselect);
		if (StopPolling)
			context.HttpContext.Response.StatusCode = HtmxStatusCodes.StopPolling;
	}

	/// <summary>
	/// Called after the action result is executed. This method is not used but is required by the interface.
	/// </summary>
	/// <param name="context">The context in which the result has been executed.</param>
	public void OnResultExecuted(ResultExecutedContext context)
	{
		// No action required
	}
}