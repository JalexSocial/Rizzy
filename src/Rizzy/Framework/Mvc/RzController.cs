using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Rizzy.Framework.Mvc;

/// <summary>
/// Base controller for Rizzy that provides access to Razor Component views
/// </summary>
public class RzController : ControllerBase, IActionFilter, IAsyncActionFilter, IDisposable
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
    /// Creates a <see cref="JsonResult"/> object that serializes the specified <paramref name="data"/> object
    /// to JSON.
    /// </summary>
    /// <param name="data">The object to serialize.</param>
    /// <returns>The created <see cref="JsonResult"/> that serializes the specified <paramref name="data"/>
    /// to JSON format for the response.</returns>
    [NonAction]
    public virtual JsonResult Json(object? data)
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
    /// When using <c>Newtonsoft.Json</c>, this should be an instance of <c>JsonSerializerSettings</c>.
    /// </para>
    /// </param>
    /// <returns>The created <see cref="JsonResult"/> that serializes the specified <paramref name="data"/>
    /// as JSON format for the response.</returns>
    /// <remarks>
    /// Callers should cache an instance of serializer settings to avoid
    /// recreating cached data with each call.
    /// </remarks>
    [NonAction]
    public virtual JsonResult Json(object? data, object? serializerSettings)
    {
        return new JsonResult(data, serializerSettings);
    }

    /// <summary>
    /// Called before the action method is invoked.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    [NonAction]
    public virtual void OnActionExecuting(ActionExecutingContext context)
    {
        HttpContext.InitializeBlazorFormData();
    }

    /// <summary>
    /// Called after the action method is invoked.
    /// </summary>
    /// <param name="context">The action executed context.</param>
    [NonAction]
    public virtual void OnActionExecuted(ActionExecutedContext context)
    {
    }

    /// <summary>
    /// Called before the action method is invoked.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    /// <param name="next">The <see cref="ActionExecutionDelegate"/> to execute. Invoke this delegate in the body
    /// of <see cref="OnActionExecutionAsync" /> to continue execution of the action.</param>
    /// <returns>A <see cref="Task"/> instance.</returns>
    [NonAction]
    public virtual Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        OnActionExecuting(context);
        if (context.Result == null)
        {
            var task = next();
            if (!task.IsCompletedSuccessfully)
            {
                return Awaited(this, task);
            }

            OnActionExecuted(task.Result);
        }

        return Task.CompletedTask;

        static async Task Awaited(RzController controller, Task<ActionExecutedContext> task)
        {
            controller.OnActionExecuted(await task);
        }
    }

    /// <inheritdoc />
    public void Dispose() => Dispose(disposing: true);

    /// <summary>
    /// Releases all resources currently used by this <see cref="Controller"/> instance.
    /// </summary>
    /// <param name="disposing"><c>true</c> if this method is being invoked by the <see cref="Dispose()"/> method,
    /// otherwise <c>false</c>.</param>
    protected virtual void Dispose(bool disposing)
    {
    }
}
