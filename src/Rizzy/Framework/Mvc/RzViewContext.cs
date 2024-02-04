using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Rizzy.Extensions;
using Rizzy.Http;

namespace Rizzy.Framework.Mvc;

public class RzViewContext
{
	public RzViewContext(HttpContext httpContext,
		RouteData routeData,
		ActionDescriptor actionDescriptor,
		ModelStateDictionary modelState,
		EditContext editContext
	)
	{
		ArgumentNullException.ThrowIfNull(httpContext);
		ArgumentNullException.ThrowIfNull(routeData);
		ArgumentNullException.ThrowIfNull(actionDescriptor);
		ArgumentNullException.ThrowIfNull(modelState);

		HttpContext = httpContext;
		RouteData = routeData;
		ActionDescriptor = actionDescriptor;
		ModelState = modelState;
		EditContext = editContext;
		Htmx = new HtmxContext(httpContext);

		EditContext.Validate();
	}

    public HtmxContext Htmx { get; init; }

    /// <summary>
    /// Gets or sets the <see cref="Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor"/> for the selected action.
    /// </summary>
    /// <remarks>
    /// The property setter is provided for unit test purposes only.
    /// </remarks>
    public ActionDescriptor ActionDescriptor { get; set; } 

    /// <summary>
    /// Gets or sets the <see cref="Microsoft.AspNetCore.Http.HttpContext"/> for the current request.
    /// </summary>
    /// <remarks>
    /// The property setter is provided for unit test purposes only.
    /// </remarks>
    public HttpContext HttpContext { get; set; }

    /// <summary>
    /// Gets the <see cref="ModelStateDictionary"/>.
    /// </summary>
    public ModelStateDictionary ModelState { get; }

    /// <summary>
    /// Gets the EditContext
    /// </summary>
    public EditContext EditContext { get; init; }

	/// <summary>
	/// Gets or sets the <see cref="AspNetCore.Routing.RouteData"/> for the current request.
	/// </summary>
	/// <remarks>
	/// The property setter is provided for unit test purposes only.
	/// </remarks>
	public Microsoft.AspNetCore.Routing.RouteData RouteData { get; set; }

    public Dictionary<string, object?> ComponentParameters { get; set; } = new();
}
