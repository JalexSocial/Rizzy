using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Rizzy.Components;
using Rizzy.Extensions;

namespace Rizzy;

/// <summary>
/// Represents a proxy base for Rizzy services that facilitate access to Razor Component views. This class provides
/// mechanisms to render both full and partial Razor views dynamically based on specified component types and parameters.
/// </summary>
public sealed class RizzyService : IRizzyService
{
    private string? _currentActionUrl;

    /// <summary>
    /// Initializes a new instance of the RizzyService with a specified view context.
    /// </summary>
    /// <param name="viewContext">The view context that provides necessary metadata for rendering Razor components.</param>
    public RizzyService(RzViewContext viewContext)
    {
        ViewContext = viewContext;
    }

    /// <summary>
    /// Gets the view context associated with the service. The view context contains information required for configuring and rendering views.
    /// </summary>
    public RzViewContext ViewContext { get; }

    /// <summary>
    /// Renders a view using the specified Razor component, optionally accepting dynamic data to pass to the component.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render.</typeparam>
    /// <param name="data">Optional dynamic data to pass to the component. Defaults to null if not provided.</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a view.</returns>
    public IResult View<TComponent>(object? data = null) where TComponent : IComponent =>
        ViewContext.Htmx.Response.EmptyResponseBodyRequested ? Results.NoContent() :
        View<TComponent>(data.ToDictionary());

    /// <summary>
    /// Renders a view using the specified Razor component with explicitly provided data in the form of a dictionary.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render.</typeparam>
    /// <param name="data">A dictionary containing the data to pass to the component.</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a view.</returns>
    public IResult View<TComponent>(Dictionary<string, object?> data) where TComponent : IComponent
    {
        if (ViewContext.Htmx.Response.EmptyResponseBodyRequested)
            return Results.NoContent();

        var parameters = new Dictionary<string, object?>();

        // Configure the view context based on the component type and provided data
        ViewContext.ConfigureView(typeof(TComponent), data);

        // Add necessary parameters for rendering the component
        parameters.Add("ComponentType", ViewContext.ComponentType);
        parameters.Add("ComponentParameters", ViewContext.ComponentParameters);
        parameters.Add("ViewContext", ViewContext);

        // Return a result that can render the component as a full page
        return new RazorComponentResult<RzPage>(parameters)
        {
            PreventStreamingRendering = false
        };
    }

    /// <summary>
    /// Renders a partial view using the specified Razor component, optionally accepting dynamic data to pass to the component.
    /// This method is intended for rendering components without a layout, suitable for inclusion in other views.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render.</typeparam>
    /// <param name="data">Optional dynamic data to pass to the component. Defaults to null if not provided.</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a partial view.</returns>
    public IResult PartialView<TComponent>(object? data = null) where TComponent : IComponent =>
        ViewContext.Htmx.Response.EmptyResponseBodyRequested ? Results.NoContent() :
        PartialView<TComponent>(data.ToDictionary());

    /// <summary>
    /// Renders a partial view using the specified Razor component with explicitly provided data in the form of a dictionary.
    /// This method is intended for rendering components without a layout, suitable for inclusion in other views.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render.</typeparam>
    /// <param name="data">A dictionary containing the data to pass to the component.</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a partial view.</returns>
    public IResult PartialView<TComponent>(Dictionary<string, object?> data) where TComponent : IComponent
    {
        if (ViewContext.Htmx.Response.EmptyResponseBodyRequested)
            return Results.NoContent();

        var parameters = new Dictionary<string, object?>();

        // Configure the view context based on the component type and provided data
        ViewContext.ConfigureView(typeof(TComponent), data);

        // Add necessary parameters for rendering the component as a partial view
        parameters.Add("ComponentType", ViewContext.ComponentType);
        parameters.Add("ComponentParameters", ViewContext.ComponentParameters);
        parameters.Add("ViewContext", ViewContext);

        // Return a result that can render the component as a partial view
        return new RazorComponentResult<RzPartial>(parameters)
        {
            PreventStreamingRendering = false
        };
    }

    /// <summary>
    /// Renders a Razor component without a layout from a RenderFragment
    /// </summary>
    /// <param name="fragment"></param>
    /// <returns></returns>
    public IResult PartialView(RenderFragment fragment)
    {
        return new RazorComponentResult<FragmentComponent>(new { Fragment = fragment });
    }

    /// <summary>
    /// Renders a Razor component without a layout from a RenderFragment
    /// </summary>
    /// <param name="fragments"></param>
    /// <returns></returns>
    public IResult PartialView(params RenderFragment[] fragments)
    {
	    return new RazorComponentResult<FragmentComponent>(new { Fragments = fragments });
    }

    /// <summary>
    /// Gets the current action method URL, which can be used as a callback URL in forms. This URL is automatically
    /// derived from the current HTTP request but can be manually overridden in form handler methods. It is particularly
    /// useful for specifying form action targets within Razor Component views.
    /// </summary>
    public string CurrentActionUrl => _currentActionUrl ??= ViewContext.HttpContext.Request.GetEncodedPathAndQuery();
}
