using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rizzy.Htmx;

namespace Rizzy;

/// <summary>
/// Represents a proxy base for Rizzy services that facilitate access to Razor Component views. This class provides
/// mechanisms to render both full and partial Razor views dynamically based on specified component types and parameters.
/// </summary>
public sealed class RizzyService : IRizzyService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string? _currentActionUrl;

    /// <summary>
    /// Initializes a new instance of the RizzyService
    /// </summary>
    /// <param name="httpContextAccessor">The HttpContextAccessor to access the scoped HttpContext.</param>
    public RizzyService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Renders a view using the specified Razor component, optionally accepting dynamic data to pass to the component.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render.</typeparam>
    /// <param name="data">Optional dynamic data to pass to the component. Defaults to null if not provided.</param>
    /// <param name="modelState">Optional MVC ModelState</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a view.</returns>
    public IResult View<TComponent>(object? data = null, ModelStateDictionary? modelState = null) where TComponent : IComponent =>
        _httpContextAccessor.HttpContext?.Response.Htmx().EmptyResponseBodyRequested == true ? Results.NoContent() :
        View<TComponent>(data.ToDictionary(), modelState);

    /// <summary>
    /// Renders a view using the specified Razor component with explicitly provided data in the form of a dictionary.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render.</typeparam>
    /// <param name="data">A dictionary containing the data to pass to the component.</param>
    /// <param name="modelState">Optional MVC ModelState</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a view.</returns>
    public IResult View<TComponent>(Dictionary<string, object?> data, ModelStateDictionary? modelState = null) where TComponent : IComponent
    {
        if (_httpContextAccessor.HttpContext?.Response.Htmx().EmptyResponseBodyRequested == true)
            return Results.NoContent();

        var parameters = new Dictionary<string, object?>
        {
            // Add necessary parameters for rendering the component
            { "ComponentType", typeof(TComponent) },
            { "ComponentParameters", data },
            { "ModelState", modelState}
        };

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
    /// <param name="modelState">Optional MVC ModelState</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a partial view.</returns>
    public IResult PartialView<TComponent>(object? data = null, ModelStateDictionary? modelState = null) where TComponent : IComponent =>
        _httpContextAccessor.HttpContext?.Response.Htmx().EmptyResponseBodyRequested == true ? Results.NoContent() :
        PartialView<TComponent>(data.ToDictionary(), modelState);

    /// <summary>
    /// Renders a partial view using the specified Razor component with explicitly provided data in the form of a dictionary.
    /// This method is intended for rendering components without a layout, suitable for inclusion in other views.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render.</typeparam>
    /// <param name="data">A dictionary containing the data to pass to the component.</param>
    /// <param name="modelState">Optional MVC ModelState</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a partial view.</returns>
    public IResult PartialView<TComponent>(Dictionary<string, object?> data, ModelStateDictionary? modelState = null) where TComponent : IComponent
    {
        if (_httpContextAccessor.HttpContext?.Response.Htmx().EmptyResponseBodyRequested == true)
            return Results.NoContent();

        var parameters = new Dictionary<string, object?>
        {
	        // Add necessary parameters for rendering the component
	        { "ComponentType", typeof(TComponent) },
            { "ComponentParameters", data },
            { "ModelState", modelState}
        };

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
    public string CurrentActionUrl => _currentActionUrl ??= _httpContextAccessor?.HttpContext?.Request.GetEncodedPathAndQuery() ?? string.Empty;
}
