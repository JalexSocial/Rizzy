using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.AspNetCore.Components.HtmlRendering.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http.Features;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web.HtmlRendering;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Web;

namespace Rizzy.Framework.Endpoints;

public class HttpResponseProxy : HttpResponse
{
	private readonly HttpResponse _originalResponse;
	private MemoryStream _stream;

	public HttpResponseProxy(HttpResponse originalResponse)
	{
		_originalResponse = originalResponse ?? throw new ArgumentNullException(nameof(originalResponse));
		_stream = new MemoryStream();
	}

	public override HttpContext HttpContext => _originalResponse.HttpContext;

	public override int StatusCode
	{
		get => _originalResponse.StatusCode;
		set => _originalResponse.StatusCode = value;
	}

	public override IHeaderDictionary Headers => _originalResponse.Headers;

	public override Stream Body
	{
		get => _originalResponse.Body;
		set => _originalResponse.Body = value;
	}

	public override long? ContentLength
	{
		get => _originalResponse.ContentLength;
		set => _originalResponse.ContentLength = value;
	}

	public override string ContentType
	{
		get => _originalResponse.ContentType;
		set => _originalResponse.ContentType = value;
	}

	public override IResponseCookies Cookies => _originalResponse.Cookies;

	public override bool HasStarted => _originalResponse.HasStarted;

	public override void OnStarting(Func<object, Task> callback, object state)
	{
		_originalResponse.OnStarting(callback, state);
	}

	public override void OnCompleted(Func<object, Task> callback, object state)
	{
		_originalResponse.OnCompleted(callback, state);
	}

	public override void Redirect(string location, bool permanent)
	{
		_originalResponse.Redirect(location, permanent);
	}

	public override Task StartAsync(CancellationToken cancellationToken = default)
	{
		return _originalResponse.StartAsync(cancellationToken);
	}

	public override Task CompleteAsync()
	{
		return _originalResponse.CompleteAsync();
	}
}

public class ProxyHttpContext : HttpContext
{
	private readonly HttpContext _originalContext;

	public ProxyHttpContext(HttpContext context)
	{
		_originalContext = context ?? throw new ArgumentNullException(nameof(context));
	}

	public override IFeatureCollection Features => _originalContext.Features;
	public override HttpRequest Request => _originalContext.Request;
	public override HttpResponse Response => new HttpResponseProxy(_originalContext.Response);
	public override ConnectionInfo Connection => _originalContext.Connection;
	public override WebSocketManager WebSockets => _originalContext.WebSockets;
	public override ClaimsPrincipal User { get => _originalContext.User; set => _originalContext.User = value; }
	public override IDictionary<object, object> Items { get => _originalContext.Items; set => _originalContext.Items = value; }
	public override IServiceProvider RequestServices { get => _originalContext.RequestServices; set => _originalContext.RequestServices = value; }
	public override CancellationToken RequestAborted { get => _originalContext.RequestAborted; set => _originalContext.RequestAborted = value; }
	public override string TraceIdentifier { get => _originalContext.TraceIdentifier; set => _originalContext.TraceIdentifier = value; }
	public override ISession Session { get => _originalContext.Session; set => _originalContext.Session = value; }

	public override void Abort()
	{
		_originalContext.Abort();
	}

	// Additional methods and properties to implement...
}

public class ExperimentalStaticHtmlRenderer : StaticHtmlRenderer
{
	private readonly IServiceProvider _services;
	private readonly RazorComponentsServiceOptions _options;
	private HttpContext? _httpContext = default!; // Always set at the start of an inbound call

	public ExperimentalStaticHtmlRenderer(IServiceProvider serviceProvider, ILoggerFactory loggerFactory) : base(serviceProvider, loggerFactory)
	{
		_services = serviceProvider;
		_options = serviceProvider.GetRequiredService<IOptions<RazorComponentsServiceOptions>>().Value;
	}

	protected override void WriteComponentHtml(int componentId, TextWriter output)
	{
		base.WriteComponentHtml(componentId, output);
	}
	
	internal HttpContext? HttpContext => _httpContext;

	private void SetHttpContext(HttpContext httpContext)
	{
		if (_httpContext is null)
		{
			_httpContext = httpContext;
		}
		else if (_httpContext != httpContext)
		{
			throw new InvalidOperationException("The HttpContext cannot change value once assigned.");
		}
	}


}


/// <summary>
/// Provides a mechanism for rendering components non-interactively as HTML markup.
/// </summary>
public sealed class ExperimentalHtmlRenderer : IDisposable, IAsyncDisposable
{
	private readonly StaticHtmlRenderer _passiveHtmlRenderer;

	/// <summary>
	/// Constructs an instance of <see cref="HtmlRenderer"/>.
	/// </summary>
	/// <param name="services">The services to use when rendering components.</param>
	/// <param name="loggerFactory">The logger factory to use.</param>
	public ExperimentalHtmlRenderer(IServiceProvider services, ILoggerFactory loggerFactory)
	{
		_passiveHtmlRenderer = new ExperimentalStaticHtmlRenderer(services, loggerFactory);
	}

	/// <inheritdoc />
	public void Dispose()
		=> _passiveHtmlRenderer.Dispose();

	/// <inheritdoc />
	public ValueTask DisposeAsync()
		=> _passiveHtmlRenderer.DisposeAsync();

	/// <summary>
	/// Gets the <see cref="Microsoft.AspNetCore.Components.Dispatcher" /> associated with this instance. Any calls to
	/// <see cref="RenderComponentAsync{TComponent}()"/> or <see cref="BeginRenderingComponent{TComponent}()"/>
	/// must be performed using this <see cref="Microsoft.AspNetCore.Components.Dispatcher" />.
	/// </summary>
	public Dispatcher Dispatcher => _passiveHtmlRenderer.Dispatcher;

	/// <summary>
	/// Adds an instance of the specified component and instructs it to render. The resulting content represents the
	/// initial synchronous rendering output, which may later change. To wait for the component hierarchy to complete
	/// any asynchronous operations such as loading, await <see cref="HtmlRootComponent.QuiescenceTask"/> before
	/// reading content from the <see cref="HtmlRootComponent"/>.
	/// </summary>
	/// <typeparam name="TComponent">The component type.</typeparam>
	/// <returns>An <see cref="HtmlRootComponent"/> instance representing the render output.</returns>
	public HtmlRootComponent BeginRenderingComponent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>() where TComponent : IComponent
		=> _passiveHtmlRenderer.BeginRenderingComponent(typeof(TComponent), ParameterView.Empty);

	/// <summary>
	/// Adds an instance of the specified component and instructs it to render. The resulting content represents the
	/// initial synchronous rendering output, which may later change. To wait for the component hierarchy to complete
	/// any asynchronous operations such as loading, await <see cref="HtmlRootComponent.QuiescenceTask"/> before
	/// reading content from the <see cref="HtmlRootComponent"/>.
	/// </summary>
	/// <typeparam name="TComponent">The component type.</typeparam>
	/// <param name="parameters">Parameters for the component.</param>
	/// <returns>An <see cref="HtmlRootComponent"/> instance representing the render output.</returns>
	public HtmlRootComponent BeginRenderingComponent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>(
		ParameterView parameters) where TComponent : IComponent
		=> _passiveHtmlRenderer.BeginRenderingComponent(typeof(TComponent), parameters);

	/// <summary>
	/// Adds an instance of the specified component and instructs it to render. The resulting content represents the
	/// initial synchronous rendering output, which may later change. To wait for the component hierarchy to complete
	/// any asynchronous operations such as loading, await <see cref="HtmlRootComponent.QuiescenceTask"/> before
	/// reading content from the <see cref="HtmlRootComponent"/>.
	/// </summary>
	/// <param name="componentType">The component type. This must implement <see cref="IComponent"/>.</param>
	/// <returns>An <see cref="HtmlRootComponent"/> instance representing the render output.</returns>
	public HtmlRootComponent BeginRenderingComponent(
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type componentType)
		=> _passiveHtmlRenderer.BeginRenderingComponent(componentType, ParameterView.Empty);

	/// <summary>
	/// Adds an instance of the specified component and instructs it to render. The resulting content represents the
	/// initial synchronous rendering output, which may later change. To wait for the component hierarchy to complete
	/// any asynchronous operations such as loading, await <see cref="HtmlRootComponent.QuiescenceTask"/> before
	/// reading content from the <see cref="HtmlRootComponent"/>.
	/// </summary>
	/// <param name="componentType">The component type. This must implement <see cref="IComponent"/>.</param>
	/// <param name="parameters">Parameters for the component.</param>
	/// <returns>An <see cref="HtmlRootComponent"/> instance representing the render output.</returns>
	public HtmlRootComponent BeginRenderingComponent(
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type componentType,
		ParameterView parameters)
		=> _passiveHtmlRenderer.BeginRenderingComponent(componentType, parameters);

	/// <summary>
	/// Adds an instance of the specified component and instructs it to render, waiting
	/// for the component hierarchy to complete asynchronous tasks such as loading.
	/// </summary>
	/// <typeparam name="TComponent">The component type.</typeparam>
	/// <returns>A task that completes with <see cref="HtmlRootComponent"/> once the component hierarchy has completed any asynchronous tasks such as loading.</returns>
	public Task<HtmlRootComponent> RenderComponentAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>() where TComponent : IComponent
		=> RenderComponentAsync<TComponent>(ParameterView.Empty);

	/// <summary>
	/// Adds an instance of the specified component and instructs it to render, waiting
	/// for the component hierarchy to complete asynchronous tasks such as loading.
	/// </summary>
	/// <param name="componentType">The component type. This must implement <see cref="IComponent"/>.</param>
	/// <returns>A task that completes with <see cref="HtmlRootComponent"/> once the component hierarchy has completed any asynchronous tasks such as loading.</returns>
	public Task<HtmlRootComponent> RenderComponentAsync(
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type componentType)
		=> RenderComponentAsync(componentType, ParameterView.Empty);

	/// <summary>
	/// Adds an instance of the specified component and instructs it to render, waiting
	/// for the component hierarchy to complete asynchronous tasks such as loading.
	/// </summary>
	/// <typeparam name="TComponent">The component type.</typeparam>
	/// <param name="parameters">Parameters for the component.</param>
	/// <returns>A task that completes with <see cref="HtmlRootComponent"/> once the component hierarchy has completed any asynchronous tasks such as loading.</returns>
	public Task<HtmlRootComponent> RenderComponentAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>(
		ParameterView parameters) where TComponent : IComponent
		=> RenderComponentAsync(typeof(TComponent), parameters);

	/// <summary>
	/// Adds an instance of the specified component and instructs it to render, waiting
	/// for the component hierarchy to complete asynchronous tasks such as loading.
	/// </summary>
	/// <param name="componentType">The component type. This must implement <see cref="IComponent"/>.</param>
	/// <param name="parameters">Parameters for the component.</param>
	/// <returns>A task that completes with <see cref="HtmlRootComponent"/> once the component hierarchy has completed any asynchronous tasks such as loading.</returns>
	public async Task<HtmlRootComponent> RenderComponentAsync(
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type componentType,
		ParameterView parameters)
	{
		var content = BeginRenderingComponent(componentType, parameters);
		await content.QuiescenceTask;
		return content;
	}
}