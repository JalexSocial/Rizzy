#nullable enable

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions; // For GetEncodedPathAndQuery
using Microsoft.AspNetCore.Http.HttpResults; // For RazorComponentResult
using Microsoft.AspNetCore.Mvc.ModelBinding; // For ModelStateDictionary
using System.Collections.Generic; // For Dictionary
using System; // For ArgumentNullException, InvalidOperationException
using Rizzy.Htmx; // For HtmxResponse and HtmxRequestHeaderNames

namespace Rizzy;

/// <summary>
/// Represents a service that facilitates rendering Blazor components as views or partial views,
/// typically within an MVC or Razor Pages application, and integrates with HTMX responses.
/// </summary>
public sealed class RizzyService : IRizzyService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string? _currentActionUrl;

    /// <summary>
    /// Initializes a new instance of the <see cref="RizzyService"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">Accessor for the current HTTP context.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="httpContextAccessor"/> is null.</exception>
    public RizzyService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    private HttpContext HttpContextSafe => _httpContextAccessor.HttpContext ??
        throw new InvalidOperationException("HttpContext is not available. Ensure IHttpContextAccessor is registered and accessible, and that this service is used within an active HTTP request scope.");

    /// <inheritdoc/>
    public string CurrentActionUrl => _currentActionUrl ??= HttpContextSafe.Request.GetEncodedPathAndQuery();

    /// <inheritdoc/>
    public IResult View<TComponent>(Action<RizzyComponentParameterBuilder<TComponent>> parameterBuilderAction, ModelStateDictionary? modelState = null)
        where TComponent : IComponent
    {
        ArgumentNullException.ThrowIfNull(parameterBuilderAction);

        if (HttpContextSafe.Response.Htmx().EmptyResponseBodyRequested)
            return Results.NoContent();

        var builderInstance = new RizzyComponentParameterBuilder<TComponent>();
        parameterBuilderAction(builderInstance); // Invoke the user's configuration lambda
        var parametersDictionary = builderInstance.Build();

        return View<TComponent>(parametersDictionary, modelState);
    }

    /// <inheritdoc/>
    public IResult View<TComponent>(object? data = null, ModelStateDictionary? modelState = null) where TComponent : IComponent =>
        View<TComponent>(data.ToDictionary(), modelState);

    /// <inheritdoc/>
    public IResult View<TComponent>(Dictionary<string, object?> data, ModelStateDictionary? modelState = null) where TComponent : IComponent
    {
        if (HttpContextSafe.Response.Htmx().EmptyResponseBodyRequested)
            return Results.NoContent();

        var parameters = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            { "ComponentType", typeof(TComponent) },
            { "ComponentParameters", data },
            { "ModelState", modelState }
        };

        return new RazorComponentResult<RzPage>(parameters)
        {
            // Streaming is generally desirable for full page views.
            PreventStreamingRendering = false
        };
    }

    /// <inheritdoc/>
    public IResult PartialView<TComponent>(Action<RizzyComponentParameterBuilder<TComponent>> parameterBuilderAction, ModelStateDictionary? modelState = null)
        where TComponent : IComponent
    {
        ArgumentNullException.ThrowIfNull(parameterBuilderAction);

        if (HttpContextSafe.Response.Htmx().EmptyResponseBodyRequested)
            return Results.NoContent();

        var builderInstance = new RizzyComponentParameterBuilder<TComponent>();
        parameterBuilderAction(builderInstance); // Invoke the user's configuration lambda
        var parametersDictionary = builderInstance.Build();

        return PartialView<TComponent>(parametersDictionary, modelState);
    }

    /// <inheritdoc/>
    public IResult PartialView<TComponent>(object? data = null, ModelStateDictionary? modelState = null) where TComponent : IComponent =>
        PartialView<TComponent>(data.ToDictionary(), modelState);

    /// <inheritdoc/>
    public IResult PartialView<TComponent>(Dictionary<string, object?> data, ModelStateDictionary? modelState = null) where TComponent : IComponent
    {
        if (HttpContextSafe.Response.Htmx().EmptyResponseBodyRequested)
            return Results.NoContent();

        var parameters = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            { "ComponentType", typeof(TComponent) },
            { "ComponentParameters", data },
            { "ModelState", modelState }
        };
        return new RazorComponentResult<RzPartial>(parameters)
        {
            // Partials are often small; streaming might be less critical or even undesirable
            // depending on HTMX swap strategies. Consider making this configurable if needed.
            PreventStreamingRendering = false
        };
    }

    /// <inheritdoc/>
    public IResult PartialView(RenderFragment fragment)
    {
        ArgumentNullException.ThrowIfNull(fragment);

        if (HttpContextSafe.Response.Htmx().EmptyResponseBodyRequested)
            return Results.NoContent();

        return new RazorComponentResult<FragmentComponent>(new { Fragment = fragment });
    }

    /// <inheritdoc/>
    public IResult PartialView(params RenderFragment[] fragments)
    {
        ArgumentNullException.ThrowIfNull(fragments);

        if (HttpContextSafe.Response.Htmx().EmptyResponseBodyRequested)
            return Results.NoContent();

        return new RazorComponentResult<FragmentComponent>(new { Fragments = fragments });
    }
}