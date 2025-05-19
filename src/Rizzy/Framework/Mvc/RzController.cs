using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Rizzy.Framework.Mvc;

/// <summary>
/// Base controller for Rizzy that provides access to Razor Component views.
/// </summary>
public class RzController : ControllerBase, IActionFilter, IAsyncActionFilter, IDisposable
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
    /// Creates a <see cref="JsonResult"/> object that serializes the specified <paramref name="data"/> object
    /// to JSON.
    /// </summary>
    /// <param name="data">The object to serialize.</param>
    /// <returns>The created <see cref="JsonResult"/> that serializes the specified <paramref name="data"/>
    /// to JSON format for the response.</returns>
    [NonAction]
    public JsonResult Json(object? data)
    {
        return new JsonResult(data);
    }

    /// <summary>
    /// Creates a <see cref="JsonResult"/> object that serializes the specified <paramref name="data"/> object
    /// to JSON.
    /// </summary>
    /// <param name="data">The object to serialize.</param>
    /// <param name="serializerSettings">
    /// The serializer settings to be used by the formatter.
    /// <para>
    /// When using <c>System.Text.Json</c>, this should be an instance of <see cref="JsonSerializerOptions" />.
    /// </para>
    /// <para>
    /// When using <c>Newtonsoft.Json</c>, this should be an instance of <c>Newtonsoft.Json.JsonSerializerSettings</c>.
    /// </para>
    /// </param>
    /// <returns>The created <see cref="JsonResult"/> that serializes the specified <paramref name="data"/>
    /// as JSON format for the response.</returns>
    /// <remarks>
    /// Callers should cache an instance of serializer settings to avoid
    /// recreating cached data with each call.
    /// </remarks>
    [NonAction]
    public JsonResult Json(object? data, object? serializerSettings)
    {
        return new JsonResult(data, serializerSettings);
    }

    /// <summary>
    /// Called before the action method is invoked. Initializes Blazor form data if the request has form content.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    [NonAction]
    public virtual void OnActionExecuting(ActionExecutingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        context.HttpContext.InitializeBlazorFormData();
    }

    /// <summary>
    /// Called after the action method is invoked.
    /// </summary>
    /// <param name="context">The action executed context.</param>
    [NonAction]
    public virtual void OnActionExecuted(ActionExecutedContext context)
    {
        // Base implementation does nothing.
    }

    /// <summary>
    /// Called before and after the action method is invoked, allowing for asynchronous operations.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    /// <param name="next">The <see cref="ActionExecutionDelegate"/> to execute. Invoke this delegate in the body
    /// of <see cref="OnActionExecutionAsync" /> to continue execution of the action.</param>
    /// <returns>A <see cref="Task"/> instance representing the asynchronous operation.</returns>
    [NonAction]
    public virtual async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        OnActionExecuting(context);
        if (context.Result == null)
        {
            var executedContext = await next();
            OnActionExecuted(executedContext);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases all resources currently used by this <see cref="RzController"/> instance.
    /// </summary>
    /// <param name="disposing"><c>true</c> if this method is being invoked by the <see cref="Dispose()"/> method,
    /// otherwise <c>false</c>.</param>
    protected virtual void Dispose(bool disposing)
    {
        // No managed resources to dispose in this base class by default.
        // Derived classes can override this if they have resources to release.
    }
}