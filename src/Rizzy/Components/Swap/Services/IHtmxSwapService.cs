using Microsoft.AspNetCore.Components;
using Rizzy.Configuration.Htmx.Enum;

namespace Rizzy.Components.Swap.Services;

public interface IHtmxSwapService
{
    event EventHandler? ContentItemsUpdated;

    /// <summary>
    /// Adds a swappable Razor component to the service for later rendering.
    /// </summary>
    /// <param name="targetId">The target DOM element ID where the component should be rendered.</param>
    /// <param name="parameters">Optional parameters to pass to the component.</param>
    /// <param name="swapStyle">The style of content swap to apply.</param>
    /// <param name="selector">A CSS selector to specify where to apply the swap.</param>
    /// <typeparam name="TComponent">The type of the Razor component to add.</typeparam>
    void AddSwappableComponent<TComponent>(string targetId, Dictionary<string, object>? parameters = null, SwapStyle swapStyle = SwapStyle.OuterHTML, string? selector = null) where TComponent : IComponent;

    /// <summary>
    /// Adds a RenderFragment to the service for later rendering.
    /// </summary>
    /// <param name="targetId">The target DOM element ID where the fragment should be rendered.</param>
    /// <param name="renderFragment">The RenderFragment to add.</param>
    /// <param name="swapStyle">The style of content swap to apply.</param>
    /// <param name="selector">A CSS selector to specify where to apply the swap.</param>
    void AddSwappableFragment(string targetId, RenderFragment renderFragment, SwapStyle swapStyle = SwapStyle.OuterHTML, string? selector = null);

    /// <summary>
    /// Adds string content to the service for later rendering.
    /// </summary>
    /// <param name="targetId">The target DOM element ID where the fragment should be rendered.</param>
    /// <param name="content">The RenderFragment to add.</param>
    /// <param name="swapStyle">The style of content swap to apply.</param>
    /// <param name="selector">A CSS selector to specify where to apply the swap.</param>
    void AddSwappableContent(string targetId, string content, SwapStyle swapStyle = SwapStyle.OuterHTML, string? selector = null);

    /// <summary>
    /// Adds raw HTML content to the service for later rendering.
    /// </summary>
    /// <param name="content">The raw HTML content to add.</param>
    void AddRawContent(string content);

    /// <summary>
    /// Renders all added content (components, fragments, and raw HTML) to a single RenderFragment.
    /// </summary>
    /// <returns>A RenderFragment containing all the content managed by the service.</returns>
    RenderFragment RenderToFragment();

    /// <summary>
    /// Renders all added content (components, fragments, and raw HTML) to a string.
    /// </summary>
    /// <returns>A string containing all the rendered content managed by the service.</returns>
    Task<string> RenderToString();

    /// <summary>
    /// Remove all content items from Swap Service
    /// </summary>
    void Clear();

    /// <summary>
    /// Is there any content available to render?
    /// </summary>
    bool ContentAvailable { get; }
}