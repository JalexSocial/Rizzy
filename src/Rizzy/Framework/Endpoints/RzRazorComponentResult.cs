// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Rizzy.Components.Swap;
using Rizzy.Components.Swap.Services;
using Rizzy.Extensions;

namespace Rizzy.Framework.Endpoints;

/// <summary>
/// An <see cref="IResult"/> that renders a Razor Component.
/// </summary>
public class RzRazorComponentResult : IResult, IStatusCodeHttpResult, IContentTypeHttpResult
{
    private static readonly IReadOnlyDictionary<string, object?> EmptyParameters
        = new Dictionary<string, object?>().AsReadOnly();

    /// <summary>
    /// Constructs an instance of <see cref="RazorComponentResult"/>.
    /// </summary>
    /// <param name="componentType">The type of the component to render. This must implement <see cref="IComponent"/>.</param>
    public RzRazorComponentResult(Type componentType)
        : this(componentType, ReadOnlyDictionary<string, object?>.Empty)
    {
    }

    /// <summary>
    /// Constructs an instance of <see cref="RzRazorComponentResult"/>.
    /// </summary>
    /// <param name="componentType">The type of the component to render. This must implement <see cref="IComponent"/>.</param>
    /// <param name="parameters">Parameters for the component.</param>
    public RzRazorComponentResult(
        Type componentType,
        object parameters)
        : this(componentType, CoerceParametersObjectToDictionary(parameters))
    {
    }

    /// <summary>
    /// Constructs an instance of <see cref="RzRazorComponentResult"/>.
    /// </summary>
    /// <param name="componentType">The type of the component to render. This must implement <see cref="IComponent"/>.</param>
    /// <param name="parameters">Parameters for the component.</param>
    public RzRazorComponentResult(
        Type componentType,
        IReadOnlyDictionary<string, object?> parameters)
    {
        ArgumentNullException.ThrowIfNull(componentType);
        ArgumentNullException.ThrowIfNull(parameters);

        // Note that the Blazor renderer will validate that componentType implements IComponent and throws a suitable
        // exception if not, so we don't need to duplicate that logic here.
        ComponentType = componentType;
        Parameters = parameters ?? EmptyParameters;
    }

    private static IReadOnlyDictionary<string, object?> CoerceParametersObjectToDictionary(object? parameters)
        => parameters is null
        ? throw new ArgumentNullException(nameof(parameters))
        : (IReadOnlyDictionary<string, object?>)parameters.ToDictionary();

    /// <summary>
    /// Gets the component type.
    /// </summary>
    public Type ComponentType { get; }

    /// <summary>
    /// Gets or sets the Content-Type header for the response.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Gets or sets the HTTP status code.
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// Gets the parameters for the component.
    /// </summary>
    public IReadOnlyDictionary<string, object?> Parameters { get; }

    /// <summary>
    /// Gets or sets a flag to indicate whether streaming rendering should be prevented. If true, the renderer will
    /// wait for the component hierarchy to complete asynchronous tasks such as loading before supplying the HTML response.
    /// If false, streaming rendering will be determined by the components being rendered.
    ///
    /// The default value is false.
    /// </summary>
    public bool PreventStreamingRendering { get; set; }

    /// <summary>
    /// Processes this result in the given <paramref name="httpContext" />.
    /// </summary>
    /// <param name="httpContext">An <see cref="HttpContext" /> associated with the current request.</param >
    /// <returns >A <see cref="T:System.Threading.Tasks.Task" /> which will complete when execution is completed.</returns >
    public Task ExecuteAsync(HttpContext httpContext)
    {
        return RenderComponent(httpContext);
    }

    private async Task RenderComponent(HttpContext httpContext)
    {
        IServiceProvider serviceProvider = httpContext.RequestServices;
        ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        IHtmxSwapService swapService = serviceProvider.GetRequiredService<IHtmxSwapService>();

        // Render the page as a razor component result
        var page = new RazorComponentResult(ComponentType, Parameters)
        {
            PreventStreamingRendering = false
        };
        
        
        // Start rendering the primary page
        await page.ExecuteAsync(httpContext);
        await httpContext.Response.BodyWriter.FlushAsync(CancellationToken.None);

        if (swapService.ContentAvailable)
        {
            // Render any additional out of band swaps
            await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);

            var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
            {
                var output = await htmlRenderer.RenderComponentAsync<HtmxSwapContent>();

                return output.ToHtmlString();
            });

            await httpContext.Response.WriteAsync(html);
            await httpContext.Response.BodyWriter.FlushAsync(CancellationToken.None);
		}
	}
}
