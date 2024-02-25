using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rizzy.Http.Attributes;

/// <summary>
/// An action filter attribute that implements <see cref="IActionConstraint"/> to ensure
/// an action method is executed only for HTMX requests.
/// </summary>
/// <remarks>
/// HTMX requests are determined by the presence of specific HTTP headers. This attribute
/// uses the <see cref="HtmxRequest"/> class to check these headers in the incoming request.
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class HtmxRequestAttribute : ActionFilterAttribute, IActionConstraint
{
	/// <inheritdoc />
	int IActionConstraint.Order => 0;

	/// <summary>
	/// Gets or sets the target element's identifier for which the action should be executed.
	/// If specified, only requests targeting this element will be accepted.
	/// </summary>
	public string? Target { get; set; }

	/// <summary>
	/// Determines whether the action method should be executed based on the characteristics of the incoming HTTP request.
	/// </summary>
	/// <param name="context">The context in which the action constraint is evaluated.</param>
	/// <returns><c>true</c> if the request is identified as an HTMX request; otherwise, <c>false</c>.</returns>
	/// <remarks>
	/// This method initializes an <see cref="HtmxRequest"/> object using the current <see cref="HttpContext"/>
	/// and then checks the <c>IsHtmx</c> property to determine if the request originates from HTMX.
	/// </remarks>
	bool IActionConstraint.Accept(ActionConstraintContext context)
	{
		// Initialize the HtmxRequest object using the current HttpContext
		var htmxRequest = new HtmxRequest(context.RouteContext.HttpContext);

		// Check if the request is from HTMX and, if a Target is specified, ensure it matches.
		bool isHtmxRequest = htmxRequest.IsHtmx;
		bool isTargetMatch = Target == null || htmxRequest.Target == Target;

		return isHtmxRequest && isTargetMatch;
	}
}
