using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Rizzy.Http;

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

    /// <summary>
    /// Gets the Htmx context for the current request.
    /// </summary>
    [Obsolete("Utilize HttpContext extension methods instead")]
    public HtmxContext Htmx => RizzyService.ViewContext.Htmx;

    /// <inheritdoc/>
    public string CurrentActionUrl => RizzyService.CurrentActionUrl;

    /// <inheritdoc/>
    public virtual IResult View<TComponent>(object? data = null) where TComponent : IComponent
        => RizzyService.View<TComponent>(data);

    /// <inheritdoc/>
    public virtual IResult View<TComponent>(Dictionary<string, object?> data) where TComponent : IComponent
        => RizzyService.View<TComponent>(data);

    /// <inheritdoc/>
    public virtual IResult PartialView<TComponent>(object? data = null) where TComponent : IComponent
        => RizzyService.PartialView<TComponent>(data);

    /// <inheritdoc/>
    public virtual IResult PartialView<TComponent>(Dictionary<string, object?> data) where TComponent : IComponent
        => RizzyService.PartialView<TComponent>(data);

    /// <inheritdoc/>
    public virtual IResult PartialView(RenderFragment fragment)
	    => RizzyService.PartialView(fragment);
}
