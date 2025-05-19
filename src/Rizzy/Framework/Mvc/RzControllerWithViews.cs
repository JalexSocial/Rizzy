using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Rizzy.Framework.Mvc;

/// <summary>
/// Base controller for Rizzy that provides access to Razor Component views and still provides standard MVC views
/// </summary>
public class RzControllerWithViews : Controller
{
    private IRizzyService? _rizzyServiceInstance; // Renamed for clarity

    /// <summary>
    /// Gets the <see cref="IRizzyService"/> instance for the current request.
    /// This service is used to render Blazor components as views or partial views.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    protected IRizzyService Rizzy => _rizzyServiceInstance ??= HttpContext.RequestServices.GetRequiredService<IRizzyService>();

    /// <summary>
    /// Returns the current action method url as a possible Form callback url but may be overridden manually in any form handler method.
    /// This value can be used inside of form Razor Component views.
    /// </summary>
    public string CurrentActionUrl => Rizzy.CurrentActionUrl;

    /// <summary>
    /// Renders a view using the specified Razor component, with parameters configured via a fluent builder.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render. Must implement <see cref="IComponent"/>.</typeparam>
    /// <param name="parameterBuilderAction">An action to configure the parameters for the component using a <see cref="RizzyComponentParameterBuilder{TComponent}"/>.</param>
    /// <param name="modelState">Optional <see cref="ModelStateDictionary"/> to provide validation state to the component.</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a view.</returns>
    [NonAction]
    public virtual IResult View<TComponent>(Action<RizzyComponentParameterBuilder<TComponent>> parameterBuilderAction, ModelStateDictionary? modelState = null)
        where TComponent : IComponent
        => Rizzy.View<TComponent>(parameterBuilderAction, modelState);

    /// <summary>
    /// Renders a view using the specified Razor component, optionally accepting dynamic data to pass to the component.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render. Must implement <see cref="IComponent"/>.</typeparam>
    /// <param name="data">Optional dynamic data to pass to the component (e.g., an anonymous object or a dictionary). Defaults to <see langword="null"/> if not provided.</param>
    /// <param name="modelState">Optional <see cref="ModelStateDictionary"/> to provide validation state to the component.</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a view.</returns>
    [NonAction]
    public virtual IResult View<TComponent>(object? data = null, ModelStateDictionary? modelState = null) where TComponent : IComponent
        => Rizzy.View<TComponent>(data, modelState);

    /// <summary>
    /// Renders a view using the specified Razor component with explicitly provided data in the form of a dictionary.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render. Must implement <see cref="IComponent"/>.</typeparam>
    /// <param name="data">A dictionary containing the data to pass to the component.</param>
    /// <param name="modelState">Optional <see cref="ModelStateDictionary"/> to provide validation state to the component.</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a view.</returns>
    [NonAction]
    public virtual IResult View<TComponent>(Dictionary<string, object?> data, ModelStateDictionary? modelState = null) where TComponent : IComponent
        => Rizzy.View<TComponent>(data, modelState);

    /// <summary>
    /// Renders a partial view using the specified Razor component, with parameters configured via a fluent builder.
    /// This method is intended for rendering components without a layout.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render. Must implement <see cref="IComponent"/>.</typeparam>
    /// <param name="parameterBuilderAction">An action to configure the parameters for the component using a <see cref="RizzyComponentParameterBuilder{TComponent}"/>.</param>
    /// <param name="modelState">Optional <see cref="ModelStateDictionary"/> to provide validation state to the component.</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a partial view.</returns>
    [NonAction]
    public virtual IResult PartialView<TComponent>(Action<RizzyComponentParameterBuilder<TComponent>> parameterBuilderAction, ModelStateDictionary? modelState = null)
        where TComponent : IComponent
        => Rizzy.PartialView<TComponent>(parameterBuilderAction, modelState);

    /// <summary>
    /// Renders a partial view using the specified Razor component, optionally accepting dynamic data to pass to the component.
    /// This method is intended for rendering components without a layout, suitable for inclusion in other views.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render. Must implement <see cref="IComponent"/>.</typeparam>
    /// <param name="data">Optional dynamic data to pass to the component (e.g., an anonymous object or a dictionary). Defaults to <see langword="null"/> if not provided.</param>
    /// <param name="modelState">Optional <see cref="ModelStateDictionary"/> to provide validation state to the component.</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a partial view.</returns>
    [NonAction]
    public virtual IResult PartialView<TComponent>(object? data = null, ModelStateDictionary? modelState = null) where TComponent : IComponent
        => Rizzy.PartialView<TComponent>(data, modelState);

    /// <summary>
    /// Renders a partial view using the specified Razor component with explicitly provided data in the form of a dictionary.
    /// This method is intended for rendering components without a layout, suitable for inclusion in other views.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component to render. Must implement <see cref="IComponent"/>.</typeparam>
    /// <param name="data">A dictionary containing the data to pass to the component.</param>
    /// <param name="modelState">Optional <see cref="ModelStateDictionary"/> to provide validation state to the component.</param>
    /// <returns>An <see cref="IResult"/> that can render the specified component as a partial view.</returns>
    [NonAction]
    public virtual IResult PartialView<TComponent>(Dictionary<string, object?> data, ModelStateDictionary? modelState = null) where TComponent : IComponent
        => Rizzy.PartialView<TComponent>(data, modelState);

    /// <summary>
    /// Renders a Razor component without a layout from a <see cref="RenderFragment"/>.
    /// </summary>
    /// <param name="fragment">The <see cref="RenderFragment"/> to render.</param>
    /// <returns>An <see cref="IResult"/> that can render the fragment as a partial view.</returns>
    [NonAction]
    public virtual IResult PartialView(RenderFragment fragment)
        => Rizzy.PartialView(fragment);

    /// <summary>
    /// Renders a Razor component without a layout from multiple <see cref="RenderFragment"/> instances.
    /// </summary>
    /// <param name="fragments">The <see cref="RenderFragment"/> array to render.</param>
    /// <returns>An <see cref="IResult"/> that can render the fragments as a partial view.</returns>
    [NonAction]
    public virtual IResult PartialView(params RenderFragment[] fragments)
        => Rizzy.PartialView(fragments);

    /// <summary>
    /// Called before the action method is invoked. Initializes Blazor form data if the request has form content.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    [NonAction]
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        context.HttpContext.InitializeBlazorFormData();
    }
}
