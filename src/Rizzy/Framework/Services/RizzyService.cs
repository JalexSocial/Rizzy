using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Rizzy.Components.Content;
using Rizzy.Extensions;
using Rizzy.Framework.Mvc;
using System.Text.Json;

namespace Rizzy.Framework.Services;

/// <summary>
/// Proxy base for Rizzy services that provide access to Razor Component views
/// </summary>
public class RizzyService : IRizzyService
{
    private string? _currentActionUrl;

    public RizzyService(RzViewContext viewContext)
    {
        ViewContext = viewContext;
    }

    public RzViewContext ViewContext { get; }

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
    public string CurrentActionUrl => _currentActionUrl ??= ViewContext.HttpContext.Request.GetEncodedPathAndQuery();
}
