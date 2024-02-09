using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace Rizzy.Framework.Services;

public interface IRizzyService
{
    RzViewContext ViewContext { get; }

    /// <summary>
    /// Returns the current action method url as a possible Form callback url but may be overridden manually in any form handler method
    /// This value can be used inside of form Razor Component views
    /// </summary>
    string CurrentActionUrl { get; }

    IResult View<TComponent>(object? data = null) where TComponent : IComponent;
    IResult View<TComponent>(Dictionary<string, object?> data) where TComponent : IComponent;
    IResult PartialView<TComponent>(object? data = null) where TComponent : IComponent;

    /// <summary>
    /// Renders a Razor component without a layout
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    IResult PartialView<TComponent>(Dictionary<string, object?> data) where TComponent : IComponent;
}