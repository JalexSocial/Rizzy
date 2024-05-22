using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rizzy.Http;

namespace Rizzy.Components;

/// <summary>
/// Service for managing dynamic content swaps in a Blazor application.
/// Allows for adding Razor components, RenderFragments, and raw HTML content,
/// and provides a mechanism to render them within a specified layout or context.
/// </summary>
public class HtmxSwapService : IHtmxSwapService
{
    private readonly HttpContext _httpContext;
    private readonly IServiceProvider _serviceProvider;
    private List<ContentItem> _contentItems = new List<ContentItem>();

    /// <summary>
    /// Triggered whenever content items are modified
    /// </summary>
    public event EventHandler? ContentItemsUpdated;

    private enum RzContentType
    {
        Swappable,
        RawHtml
    }

    public HtmxSwapService(IHttpContextAccessor contextAccessor, IServiceProvider serviceProvider)
    {
        _httpContext = contextAccessor.HttpContext!;
        _serviceProvider = serviceProvider;
    }

    private record struct ContentItem(RzContentType ContentType, string TargetId, SwapStyle SwapStyle, string Selector, RenderFragment Content);

    /// <summary>
    /// Notifies ContentItemsUpdated event handler of change to content items
    /// </summary>
    protected virtual void OnContentItemsUpdated()
    {
        ContentItemsUpdated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Adds a swappable Razor component to the service for later rendering.
    /// </summary>
    /// <param name="targetId">The target DOM element ID where the component should be rendered.</param>
    /// <param name="parameters">Optional parameters to pass to the component.</param>
    /// <param name="swapStyle">The style of content swap to apply.</param>
    /// <param name="selector">A CSS selector to specify where to apply the swap.</param>
    /// <typeparam name="TComponent">The type of the Razor component to add.</typeparam>
    public void AddSwappableComponent<TComponent>(string targetId, Dictionary<string, object>? parameters = null, SwapStyle swapStyle = SwapStyle.outerHTML, string? selector = null) where TComponent : IComponent
    {
        var component = new RenderFragment(builder =>
        {
            builder.OpenComponent(0, typeof(TComponent));
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    builder.AddAttribute(1, parameter.Key, parameter.Value);
                }
            }
            builder.CloseComponent();
        });

        _contentItems.Add(new ContentItem(RzContentType.Swappable, targetId, swapStyle, selector ?? string.Empty, component));

        OnContentItemsUpdated();
    }

    /// <summary>
    /// Adds a RenderFragment to the service for later rendering.
    /// </summary>
    /// <param name="targetId">The target DOM element ID where the fragment should be rendered.</param>
    /// <param name="renderFragment">The RenderFragment to add.</param>
    /// <param name="swapStyle">The style of content swap to apply.</param>
    /// <param name="selector">A CSS selector to specify where to apply the swap.</param>
    public void AddSwappableFragment(string targetId, RenderFragment renderFragment, SwapStyle swapStyle = SwapStyle.outerHTML, string? selector = null)
    {
        _contentItems.Add(new ContentItem(RzContentType.Swappable, targetId, swapStyle, selector ?? string.Empty, renderFragment));

        OnContentItemsUpdated();
    }

    /// <summary>
    /// Adds string content to the service for later rendering.
    /// </summary>
    /// <param name="targetId">The target DOM element ID where the fragment should be rendered.</param>
    /// <param name="content">The RenderFragment to add.</param>
    /// <param name="swapStyle">The style of content swap to apply.</param>
    /// <param name="selector">A CSS selector to specify where to apply the swap.</param>
    public void AddSwappableContent(string targetId, string content, SwapStyle swapStyle = SwapStyle.outerHTML, string? selector = null)
    {
        var contentFragment = new RenderFragment(builder => builder.AddMarkupContent(1, content));

        _contentItems.Add(new ContentItem(RzContentType.Swappable, targetId, swapStyle, selector ?? string.Empty, contentFragment));

        OnContentItemsUpdated();
    }

    /// <summary>
    /// Adds raw HTML content to the service for later rendering.
    /// </summary>
    /// <param name="content">The raw HTML content to add.</param>
    public void AddRawContent(string content)
    {
        var contentFragment = new RenderFragment(builder => builder.AddMarkupContent(2, content));
        _contentItems.Add(new ContentItem(RzContentType.RawHtml, string.Empty, SwapStyle.none, string.Empty, contentFragment));

        OnContentItemsUpdated();
    }

    /// <summary>
    /// Renders all added content (components, fragments, and raw HTML) to a single RenderFragment.
    /// </summary>
    /// <returns>A RenderFragment containing all the content managed by the service.</returns>
    public RenderFragment RenderToFragment()
    {
        var isHtmx = new HtmxRequest(_httpContext).IsHtmx;

        return builder =>
        {
            foreach (var item in _contentItems)
            {
                if (item.ContentType == RzContentType.RawHtml)
                {
                    builder.AddContent(3, item.Content);
                }
                else if (item.ContentType == RzContentType.Swappable && isHtmx)
                {
                    // OpenComponent sequence number is set to a fixed value, as each component is unique in this context
                    builder.OpenComponent(3, typeof(HtmxSwappable));

                    // Attributes are added with explicit sequence numbers
                    builder.AddAttribute(4, "TargetId", item.TargetId);
                    builder.AddAttribute(5, "SwapStyle", item.SwapStyle);
                    builder.AddAttribute(6, "Selector", item.Selector);
                    builder.AddAttribute(7, "ChildContent", item.Content);

                    builder.CloseComponent();
                }
            }
        };
    }

    /// <summary>
    /// Renders all added content (components, fragments, and raw HTML) to a string.
    /// </summary>
    /// <returns>A string containing all the rendered content managed by the service.</returns>
    public async Task<string> RenderToString()
    {
        var content = string.Empty;

        if (ContentAvailable)
        {
            ILoggerFactory loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();

            await using var renderer = new HtmlRenderer(_serviceProvider, loggerFactory);

            // Render any additional out of band swaps
            content = await renderer.Dispatcher.InvokeAsync(async () =>
            {
                var output = await renderer.RenderComponentAsync<HtmxSwapContent>();

                return output.ToHtmlString();
            });
        }

        return content;
    }

    /// <summary>
    /// Remove all content items from Swap Service
    /// </summary>
    public void Clear()
    {
        _contentItems.Clear();
    }

    /// <inheritdoc/>
    public bool ContentAvailable => _contentItems.Count > 0;
}
