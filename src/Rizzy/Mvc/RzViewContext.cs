using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Rizzy.Http;

namespace Rizzy.Mvc;

public class RzViewContext
{
    public RzViewContext(HttpContext httpContext,
        RouteData routeData,
        ActionDescriptor actionDescriptor,
        ModelStateDictionary modelState
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
        Htmx = new HtmxContext(httpContext);
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
    /// Gets or sets the <see cref="AspNetCore.Routing.RouteData"/> for the current request.
    /// </summary>
    /// <remarks>
    /// The property setter is provided for unit test purposes only.
    /// </remarks>
    public Microsoft.AspNetCore.Routing.RouteData RouteData { get; set; }
}
