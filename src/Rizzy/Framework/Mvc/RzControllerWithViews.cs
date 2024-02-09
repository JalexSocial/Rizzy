using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Rizzy.Components.Content;
using Rizzy.Extensions;
using Rizzy.Framework.Services;
using System.Text.Json;

namespace Rizzy.Framework.Mvc;

/// <summary>
/// Base controller for Rizzy that provides access to Razor Component views and still provides standard MVC views
/// </summary>
public class RzControllerWithViews : Controller, IRizzyService
{
    private readonly IRizzyService? _serviceProxy = null;
    private IRizzyService RizzyService => _serviceProxy ?? this.HttpContext.RequestServices.GetRequiredService<IRizzyService>();

    /// <inheritdoc/>
    public RzViewContext ViewContext => RizzyService.ViewContext;

    /// <inheritdoc/>
    public string CurrentActionUrl => RizzyService.CurrentActionUrl;

    /// <inheritdoc/>
    public IResult View<TComponent>(object? data = null) where TComponent : IComponent 
        => RizzyService.View<TComponent>(data);

    /// <inheritdoc/>
    public IResult View<TComponent>(Dictionary<string, object?> data) where TComponent : IComponent
        => RizzyService.View<TComponent>(data);

    /// <inheritdoc/>
    public IResult PartialView<TComponent>(object? data = null) where TComponent : IComponent
        => RizzyService.PartialView<TComponent>(data);

    /// <inheritdoc/>
    public IResult PartialView<TComponent>(Dictionary<string, object?> data) where TComponent : IComponent
        => RizzyService.PartialView<TComponent>(data);

    /// <summary>
    /// Called before the action method is invoked.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    [NonAction]
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ViewContext.ConfigureActionContext(Url.ActionContext);

        base.OnActionExecuting(context);
    }
}
