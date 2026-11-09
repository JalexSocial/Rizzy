using Microsoft.AspNetCore.Components;
using Rizzy.Htmx;

namespace Rizzy;

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
    void AddSwappableComponent<TComponent>(string targetId, Dictionary<string, object?>? parameters = null, SwapStyle swapStyle = SwapStyle.outerHTML, string? selector = null) where TComponent : IComponent;

    /// <summary>
    /// Adds a swappable Razor component to the service for later rendering.
    /// </summary>
    /// <param name="targetId">The target DOM element ID where the component should be rendered.</param>
    /// <param name="parameters">Optional parameters to pass to the component.</param>
    /// <param name="swapStyle">The style of content swap to apply.</param>
    /// <param name="selector">A CSS selector to specify where to apply the swap.</param>
    /// <typeparam name="TComponent">The type of the Razor component to add.</typeparam>
    void AddSwappableComponent<TComponent>(string targetId, object? parameters = null, SwapStyle swapStyle = SwapStyle.outerHTML, string? selector = null) where TComponent : IComponent;

    /// <summary>
    /// Adds a RenderFragment to the service for later rendering.
    /// </summary>
    /// <param name="targetId">The target DOM element ID where the fragment should be rendered.</param>
    /// <param name="renderFragment">The RenderFragment to add.</param>
    /// <param name="swapStyle">The style of content swap to apply.</param>
    /// <param name="selector">A CSS selector to specify where to apply the swap.</param>
    void AddSwappableFragment(string targetId, RenderFragment renderFragment, SwapStyle swapStyle = SwapStyle.outerHTML, string? selector = null);

    /// <summary>
    /// Adds pre-rendered HTML to swap into the target. The markup is inserted as-is: never pass
    /// unencoded user input. Use <see cref="AddSwappableText"/> for plain text.
    /// </summary>
    /// <param name="targetId">The target element id (escaped automatically).</param>
    /// <param name="markup">The HTML to insert, for example <c>(MarkupString)html</c>.</param>
    /// <param name="swapStyle">The swap style.</param>
    /// <param name="selector">An optional CSS selector to use instead of the id.</param>
    void AddSwappableContent(string targetId, MarkupString markup, SwapStyle swapStyle = SwapStyle.outerHTML, string? selector = null);

    /// <summary>
    /// Adds plain text to swap into the target. The text is HTML-encoded.
    /// </summary>
    /// <param name="targetId">The target element id (escaped automatically).</param>
    /// <param name="text">The text to insert.</param>
    /// <param name="swapStyle">The swap style.</param>
    /// <param name="selector">An optional CSS selector to use instead of the id.</param>
    void AddSwappableText(string targetId, string text, SwapStyle swapStyle = SwapStyle.innerHTML, string? selector = null);

    /// <summary>
    /// Adds HTML that is appended to the response outside of any swap target (also for non-htmx
    /// requests). The markup is inserted as-is: never pass unencoded user input.
    /// </summary>
    /// <param name="markup">The HTML to insert, for example <c>(MarkupString)html</c>.</param>
    void AddRawContent(MarkupString markup);

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