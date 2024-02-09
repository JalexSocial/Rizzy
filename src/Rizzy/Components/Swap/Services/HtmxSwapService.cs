using Microsoft.AspNetCore.Components;
using Rizzy.Configuration.Htmx.Enum;

namespace Rizzy.Components.Swap.Services;

/// <summary>
/// Service for managing dynamic content swaps in a Blazor application.
/// Allows for adding Razor components, RenderFragments, and raw HTML content,
/// and provides a mechanism to render them within a specified layout or context.
/// </summary>
public class HtmxSwapService : IHtmxSwapService
{
    private List<ContentItem> _contentItems = new List<ContentItem>();
    private enum RzContentType
    {
        Swappable,
        RawHtml
    }

    private record struct ContentItem(RzContentType ContentType, string TargetId, SwapStyle SwapStyle, string Selector, RenderFragment Content);

    /// <summary>
    /// Adds a swappable Razor component to the service for later rendering.
    /// </summary>
    /// <param name="targetId">The target DOM element ID where the component should be rendered.</param>
    /// <param name="swapStyle">The style of content swap to apply.</param>
    /// <param name="selector">A CSS selector to specify where to apply the swap.</param>
    /// <param name="parameters">Optional parameters to pass to the component.</param>
    /// <typeparam name="TComponent">The type of the Razor component to add.</typeparam>
    public void AddSwappableComponent<TComponent>(string targetId, SwapStyle swapStyle, string selector, Dictionary<string, object>? parameters = null) where TComponent : IComponent
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

        _contentItems.Add(new ContentItem(RzContentType.Swappable, targetId, swapStyle, selector, component));
    }

    /// <summary>
    /// Adds a RenderFragment to the service for later rendering.
    /// </summary>
    /// <param name="targetId">The target DOM element ID where the fragment should be rendered.</param>
    /// <param name="swapStyle">The style of content swap to apply.</param>
    /// <param name="selector">A CSS selector to specify where to apply the swap.</param>
    /// <param name="renderFragment">The RenderFragment to add.</param>
    public void AddSwappableFragment(string targetId, SwapStyle swapStyle, string selector, RenderFragment renderFragment)
    {
        _contentItems.Add(new ContentItem(RzContentType.Swappable, targetId, swapStyle, selector, renderFragment));
    }

    /// <summary>
    /// Adds raw HTML content to the service for later rendering.
    /// </summary>
    /// <param name="content">The raw HTML content to add.</param>
    public void AddRawContent(string content)
    {
        var contentFragment = new RenderFragment(builder => builder.AddMarkupContent(0, content));
        _contentItems.Add(new ContentItem(RzContentType.RawHtml, string.Empty, SwapStyle.None, string.Empty, contentFragment));
    }

    /// <summary>
    /// Renders all added content (components, fragments, and raw HTML) to a single RenderFragment.
    /// </summary>
    /// <returns>A RenderFragment containing all the content managed by the service.</returns>
    public RenderFragment RenderToFragment()
    {
        return builder =>
        {
            foreach (var item in _contentItems)
            {
                if (item.ContentType == RzContentType.RawHtml)
                {
                    builder.AddContent(0, item.Content);
                }
                else if (item.ContentType == RzContentType.Swappable)
                {
                    // OpenComponent sequence number is set to a fixed value, as each component is unique in this context
                    builder.OpenComponent(0, typeof(HtmxSwappable));

                    // Attributes are added with explicit sequence numbers
                    builder.AddAttribute(1, "TargetId", item.TargetId);
                    builder.AddAttribute(2, "SwapStyle", item.SwapStyle);
                    builder.AddAttribute(3, "Selector", item.Selector);
                    builder.AddAttribute(4, "ChildContent", item.Content);

                    builder.CloseComponent();
                }
            }
            _contentItems.Clear();
        };
    }

    /// <inheritdoc/>
    public bool ContentAvailable => _contentItems.Count > 0;
}
