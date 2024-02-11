// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Infrastructure;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Rizzy.Components.Layout;
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
	/*
	internal static async Task InitializeStandardComponentServicesAsync(
			HttpContext httpContext,
			Type? componentType = null,
			string? handler = null,
			IFormCollection? form = null)
	{
		var navigationManager = (IHostEnvironmentNavigationManager)httpContext.RequestServices.GetRequiredService<NavigationManager>();
		navigationManager?.Initialize(GetContextBaseUri(httpContext.Request), GetFullUri(httpContext.Request));

		if (httpContext.RequestServices.GetService<AuthenticationStateProvider>() is IHostEnvironmentAuthenticationStateProvider authenticationStateProvider)
		{
			var authenticationState = new AuthenticationState(httpContext.User);
			authenticationStateProvider.SetAuthenticationState(Task.FromResult(authenticationState));
		}

		if (handler != null && form != null)
		{
			var fdpType = Type.GetType("Microsoft.AspNetCore.Components.Endpoints.HttpContextFormDataProvider");

			var fdp = httpContext.RequestServices.GetRequiredService(fdpType!);


				//.SetFormData(handler, new FormCollectionReadOnlyDictionary(form), form.Files);
		}

		// It's important that this is initialized since a component might try to restore state during prerendering
		// (which will obviously not work, but should not fail)
		var componentApplicationLifetime = httpContext.RequestServices.GetRequiredService<ComponentStatePersistenceManager>();
		await componentApplicationLifetime.RestoreStateAsync(new PrerenderComponentApplicationStore());
	}
	*/
	internal class EmptyComponent : LayoutComponentBase
	{
		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
		}
	}

	private async Task RenderComponent(HttpContext httpContext)
    {
        IServiceProvider serviceProvider = httpContext.RequestServices;
        ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        IHtmxSwapService swapService = serviceProvider.GetRequiredService<IHtmxSwapService>();

		// Render an empty component to force blazor to initialize
        await new RazorComponentResult(typeof(EmptyComponent)).ExecuteAsync(httpContext);

		/*
        // Render the page as a razor component result
        var page = new RazorComponentResult(ComponentType, Parameters)
        {
            PreventStreamingRendering = false
        };
        
        
        // Start rendering the primary page
        await page.ExecuteAsync(httpContext);
        await httpContext.Response.BodyWriter.FlushAsync(CancellationToken.None);
        */

		await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);
		var rootComponent = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
		{
			var parameters = ParameterView.FromDictionary(Parameters.ToDictionary());
			var component = htmlRenderer.BeginRenderingComponent(ComponentType, parameters);
			var firstRender = component.ToHtmlString();
			var output = string.Empty;

			// Give up to 50ms for component to render and then defer to lazy loading
			await Task.WhenAny(component.QuiescenceTask, Task.Delay(50));

			if (!component.QuiescenceTask.IsCompleted)
			{
				await httpContext.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(firstRender));
				await httpContext.Response.BodyWriter.FlushAsync();

				await component.QuiescenceTask;
				output = component.ToHtmlString();

				await httpContext.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(output));
				await httpContext.Response.BodyWriter.FlushAsync();
			}
			else
			{
				output = component.ToHtmlString();

				await httpContext.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(output));
				await httpContext.Response.BodyWriter.FlushAsync();
			}

			return component;
		});


		if (swapService.ContentAvailable)
        {
            // Render any additional out of band swaps

            var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
            {
                var output = await htmlRenderer.RenderComponentAsync<HtmxSwapContent>();

                return output.ToHtmlString();
            });

            await httpContext.Response.WriteAsync(html);
            await httpContext.Response.BodyWriter.FlushAsync(CancellationToken.None);
		}
	}

	private static string GetFullUri(HttpRequest request)
	{
		return UriHelper.BuildAbsolute(
			request.Scheme,
			request.Host,
			request.PathBase,
			request.Path,
			request.QueryString);
	}

	private static string GetContextBaseUri(HttpRequest request)
	{
		var result = UriHelper.BuildAbsolute(request.Scheme, request.Host, request.PathBase);

		// PathBase may be "/" or "/some/thing", but to be a well-formed base URI
		// it has to end with a trailing slash
		return result.EndsWith('/') ? result : result += "/";
	}

	private sealed class FormCollectionReadOnlyDictionary : IReadOnlyDictionary<string, StringValues>
	{
		private readonly IFormCollection _form;
		private List<StringValues>? _values;

		public FormCollectionReadOnlyDictionary(IFormCollection form)
		{
			_form = form;
		}

		public StringValues this[string key] => _form[key];

		public IEnumerable<string> Keys => _form.Keys;

		public IEnumerable<StringValues> Values => _values ??= MaterializeValues(_form);

		private static List<StringValues> MaterializeValues(IFormCollection form)
		{
			var result = new List<StringValues>(form.Keys.Count);
			foreach (var key in form.Keys)
			{
				result.Add(form[key]);
			}

			return result;
		}

		public int Count => _form.Count;

		public bool ContainsKey(string key)
		{
			return _form.ContainsKey(key);
		}

		public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
		{
			return _form.GetEnumerator();
		}

		public bool TryGetValue(string key, [MaybeNullWhen(false)] out StringValues value)
		{
			return _form.TryGetValue(key, out value);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _form.GetEnumerator();
		}
	}
}

#pragma warning disable CA1852 // Seal internal types
internal class PrerenderComponentApplicationStore : IPersistentComponentStateStore
#pragma warning restore CA1852 // Seal internal types
{
	private bool _stateIsPersisted;

	public PrerenderComponentApplicationStore()
	{
		ExistingState = new();
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "Simple deserialize of primitive types.")]
	public PrerenderComponentApplicationStore(string existingState)
	{
		ArgumentNullException.ThrowIfNull(existingState);

		DeserializeState(Convert.FromBase64String(existingState));
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "Simple deserialize of primitive types.")]
	protected void DeserializeState(byte[] existingState)
	{
		var state = JsonSerializer.Deserialize<Dictionary<string, byte[]>>(existingState);
		if (state == null)
		{
			throw new ArgumentException("Could not deserialize state correctly", nameof(existingState));
		}

		ExistingState = state;
	}

#nullable enable
	public string? PersistedState { get; private set; }
#nullable disable

	public Dictionary<string, byte[]> ExistingState { get; protected set; }

	public Task<IDictionary<string, byte[]>> GetPersistedStateAsync()
	{
		return Task.FromResult((IDictionary<string, byte[]>)ExistingState);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "Simple serialize of primitive types.")]
	protected virtual byte[] SerializeState(IReadOnlyDictionary<string, byte[]> state) =>
		JsonSerializer.SerializeToUtf8Bytes(state);

	public Task PersistStateAsync(IReadOnlyDictionary<string, byte[]> state)
	{
		if (_stateIsPersisted)
		{
			throw new InvalidOperationException("State already persisted.");
		}

		_stateIsPersisted = true;

		if (state is not null && state.Count > 0)
		{
			PersistedState = Convert.ToBase64String(SerializeState(state));
		}

		return Task.CompletedTask;
	}

	public virtual bool SupportsRenderMode(IComponentRenderMode renderMode) =>
		renderMode is null || renderMode is InteractiveWebAssemblyRenderMode || renderMode is InteractiveAutoRenderMode;
}