using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Rizzy.Components.Content;
using Rizzy.Extensions;
using System.Text.Json;

namespace Rizzy.Framework.Mvc;

/// <summary>
/// Base controller for Rizzy that provides access to Razor Component views
/// </summary>
public class RzController : ControllerBase, IActionFilter, IAsyncActionFilter, IDisposable
{
    private string? _currentActionUrl;
    private RzViewContext? _viewContext = null;

    public RzViewContext ViewContext
    {
        get
        {
            if (_viewContext != null)
                return _viewContext;

            _viewContext = HttpContext.RequestServices.GetRequiredService<RzViewContext>();
            _viewContext.ConfigureActionContext(Url.ActionContext);

            return _viewContext;
        }
    }

    public IResult View<TComponent>(object? data = null) where TComponent : IComponent =>
        View<TComponent>(data.ToDictionary());

    public IResult View<TComponent>(Dictionary<string, object?> data) where TComponent : IComponent
    {
        var parameters = new Dictionary<string, object?>();

        ViewContext.ConfigureView(typeof(TComponent), data);

        parameters.Add("ComponentType", ViewContext.ComponentType);
        parameters.Add("ComponentParameters", ViewContext.ComponentParameters);
        parameters.Add("ViewContext", ViewContext);

        return new RazorComponentResult<RzPage>(parameters)
        {
            PreventStreamingRendering = false
        };
    }

    public IResult PartialView<TComponent>(object? data = null) where TComponent : IComponent =>
        PartialView<TComponent>(data.ToDictionary());

    /// <summary>
    /// Renders a Razor component without a layout
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public IResult PartialView<TComponent>(Dictionary<string, object?> data) where TComponent : IComponent
    {
        var parameters = new Dictionary<string, object?>();

        ViewContext.ConfigureView(typeof(TComponent), data);

        parameters.Add("ComponentType", ViewContext.ComponentType);
        parameters.Add("ComponentParameters", ViewContext.ComponentParameters);
        parameters.Add("ViewContext", ViewContext);

        return new RazorComponentResult<RzPartial>(parameters)
        {
            PreventStreamingRendering = false
        };
    }

    /// <summary>
    /// Returns the current action method url as a possible Form callback url but may be overridden manually in any form handler method
    /// This value can be used inside of form Razor Component views
    /// </summary>
    public string CurrentActionUrl => _currentActionUrl ??= Request.GetEncodedPathAndQuery();

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
    /// <param name="serializerSettings">The serializer settings to be used by the formatter.
    /// <para>
    /// When using <c>System.Text.Json</c>, this should be an instance of <see cref="JsonSerializerOptions" />.
    /// </para>
    /// <para>
    /// When using <c>Newtonsoft.Json</c>, this should be an instance of <c>JsonSerializerSettings</c>.
    /// </para>
    /// </param>
    /// <returns>The created <see cref="JsonResult"/> that serializes the specified <paramref name="data"/>
    /// as JSON format for the response.</returns>
    /// <remarks>Callers should cache an instance of serializer settings to avoid
    /// recreating cached data with each call.</remarks>
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
