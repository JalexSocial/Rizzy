using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace Rizzy;

public interface IRizzyService
{
    /// <summary>
    /// Gets the view context associated with the service. The view context contains information required for configuring and rendering views.
    /// </summary>
    RzViewContext ViewContext { get; }

    /// <summary>
    /// Returns the current action method url as a possible Form callback url but may be overridden manually in any form handler method
    /// This value can be used inside of form Razor Component views
    /// </summary>
    string CurrentActionUrl { get; }

    /// <summary>
    /// Renders a view using the specified Razor component, optionally accepting dynamic data to pass to the component.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render.</typeparam>
    /// <param name="data">Optional dynamic data to pass to the component. Defaults to null if not provided.</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a view.</returns>
    IResult View<TComponent>(object? data = null) where TComponent : IComponent;

    /// <summary>
    /// Renders a view using the specified Razor component with explicitly provided data in the form of a dictionary.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render.</typeparam>
    /// <param name="data">A dictionary containing the data to pass to the component.</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a view.</returns>
    IResult View<TComponent>(Dictionary<string, object?> data) where TComponent : IComponent;

    /// <summary>
    /// Renders a partial view using the specified Razor component, optionally accepting dynamic data to pass to the component.
    /// This method is intended for rendering components without a layout, suitable for inclusion in other views.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render.</typeparam>
    /// <param name="data">Optional dynamic data to pass to the component. Defaults to null if not provided.</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a partial view.</returns>
    IResult PartialView<TComponent>(object? data = null) where TComponent : IComponent;

    /// <summary>
    /// Renders a Razor component without a layout
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    IResult PartialView<TComponent>(Dictionary<string, object?> data) where TComponent : IComponent;

    /// <summary>
    /// Renders a Razor component without a layout from a RenderFragment
    /// </summary>
    /// <param name="fragment"></param>
    /// <returns></returns>
    IResult PartialView(RenderFragment fragment);
}