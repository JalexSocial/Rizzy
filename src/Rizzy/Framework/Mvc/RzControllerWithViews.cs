using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Rizzy.Htmx;

namespace Rizzy.Framework.Mvc;

/// <summary>
/// Base controller for Rizzy that provides access to Razor Component views and still provides standard MVC views
/// </summary>
public class RzControllerWithViews : Controller 
{
    private readonly IRizzyService? _serviceProxy = null;
    private IRizzyService RizzyService => _serviceProxy ?? this.HttpContext.RequestServices.GetRequiredService<IRizzyService>();

    /// <summary>
    /// Returns the current action method url as a possible Form callback url but may be overridden manually in any form handler method.
    /// This value can be used inside of form Razor Component views.
    /// </summary>
    public string CurrentActionUrl => RizzyService.CurrentActionUrl;

    /// <summary>
    /// Renders a view using the specified Razor component, optionally accepting dynamic data to pass to the component.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render.</typeparam>
    /// <param name="data">Optional dynamic data to pass to the component. Defaults to null if not provided.</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a view.</returns>
    public virtual IResult View<TComponent>(object? data = null) where TComponent : IComponent
        => RizzyService.View<TComponent>(data, ModelState);

    /// <summary>
    /// Renders a view using the specified Razor component with explicitly provided data in the form of a dictionary.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render.</typeparam>
    /// <param name="data">A dictionary containing the data to pass to the component.</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a view.</returns>
    public virtual IResult View<TComponent>(Dictionary<string, object?> data) where TComponent : IComponent
        => RizzyService.View<TComponent>(data, ModelState);

    /// <summary>
    /// Renders a partial view using the specified Razor component, optionally accepting dynamic data to pass to the component.
    /// This method is intended for rendering components without a layout, suitable for inclusion in other views.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render.</typeparam>
    /// <param name="data">Optional dynamic data to pass to the component. Defaults to null if not provided.</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a partial view.</returns>
    public virtual IResult PartialView<TComponent>(object? data = null) where TComponent : IComponent
        => RizzyService.PartialView<TComponent>(data, ModelState);

    /// <summary>
    /// Renders a Razor component without a layout.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render.</typeparam>
    /// <param name="data">A dictionary containing the data to pass to the component.</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a partial view.</returns>
    public virtual IResult PartialView<TComponent>(Dictionary<string, object?> data) where TComponent : IComponent
        => RizzyService.PartialView<TComponent>(data, ModelState);

    /// <summary>
    /// Renders a Razor component without a layout from a <see cref="RenderFragment"/>.
    /// </summary>
    /// <param name="fragment">The <see cref="RenderFragment"/> to render.</param>
    /// <returns>An <see cref="IResult"/> that can render the fragment as a partial view.</returns>
    public virtual IResult PartialView(RenderFragment fragment)
        => RizzyService.PartialView(fragment);

    /// <summary>
    /// Renders a Razor component without a layout from multiple <see cref="RenderFragment"/> instances.
    /// </summary>
    /// <param name="fragments">The <see cref="RenderFragment"/> array to render.</param>
    /// <returns>An <see cref="IResult"/> that can render the fragments as a partial view.</returns>
    public virtual IResult PartialView(params RenderFragment[] fragments)
        => RizzyService.PartialView(fragments);

    /// <summary>
    /// Called before the action method is invoked.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    [NonAction]
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        HttpContext.InitializeBlazorFormData();
    }
}
