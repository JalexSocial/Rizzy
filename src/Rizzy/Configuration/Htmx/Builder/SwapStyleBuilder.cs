using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rizzy.Configuration.Htmx.Enum;
using String = System.String;

namespace Rizzy.Configuration.Htmx.Builder;

/// <summary>
/// A builder class for constructing a swap style command string for HTMX responses.
/// </summary>
public class SwapStyleBuilder
{
    private SwapStyle _style;
    private List<string> _commands = new List<string>();

    /// <summary>
    /// Initializes a new instance of the SwapStyleBuilder with a specified swap style.
    /// </summary>
    /// <param name="style">The initial swap style to be applied.</param>
    public SwapStyleBuilder(SwapStyle style)
    {
        _style = style;
    }

    /// <summary>
    /// Adds a delay to the swap operation.
    /// </summary>
    /// <param name="span">The time span to delay the swap.</param>
    /// <returns>The SwapStyleBuilder instance for chaining.</returns>
    public SwapStyleBuilder After(TimeSpan span)
    {
        _commands.Add(span.TotalMilliseconds < 1000 ? $"swap:{span.TotalMilliseconds}ms" : $"swap:{span.TotalSeconds}s");
        return this;
    }

    /// <summary>
    /// Specifies the direction to scroll the page after the swap.
    /// </summary>
    /// <param name="direction">The scroll direction.</param>
    /// <returns>The SwapStyleBuilder instance for chaining.</returns>
    public SwapStyleBuilder Scroll(ScrollDirection direction)
    {
        _commands.Add($"scroll:{direction.ToString().ToLowerInvariant()}");
        return this;
    }

    /// <summary>
    /// Determines whether to ignore the document title in the swap response.
    /// </summary>
    /// <param name="ignore">Whether to ignore the title.</param>
    /// <returns>The SwapStyleBuilder instance for chaining.</returns>
    public SwapStyleBuilder IgnoreTitle(bool ignore)
    {
        _commands.Add($"ignoreTitle:{ignore}");
        return this;
    }

    /// <summary>
    /// Enables or disables transition effects for the swap.
    /// </summary>
    /// <param name="show">Whether to show transition effects.</param>
    /// <returns>The SwapStyleBuilder instance for chaining.</returns>
    public SwapStyleBuilder Transition(bool show)
    {
        _commands.Add($"transition:{show}");
        return this;
    }

    /// <summary>
    /// Sets whether to focus and scroll to the swapped content.
    /// </summary>
    /// <param name="scroll">Whether to scroll to the focus element.</param>
    /// <returns>The SwapStyleBuilder instance for chaining.</returns>
    public SwapStyleBuilder FocusScroll(bool scroll)
    {
        _commands.Add($"focus-scroll:{scroll}");
        return this;
    }

    /// <summary>
    /// Specifies a CSS selector to dynamically target for the swap operation.
    /// </summary>
    /// <param name="selector">The CSS selector of the target element.</param>
    /// <param name="direction">The scroll direction after swap.</param>
    /// <returns>The SwapStyleBuilder instance for chaining.</returns>
    public SwapStyleBuilder ShowOn(string selector, ScrollDirection direction)
    {
        _commands.Add($"show:{selector}:{direction.ToString().ToLowerInvariant()}");
        return this;
    }

    /// <summary>
    /// Specifies that the swap should show in the window, with an optional scroll direction.
    /// </summary>
    /// <param name="direction">The direction to scroll the window after the swap.</param>
    /// <returns>The SwapStyleBuilder instance for chaining.</returns>
    public SwapStyleBuilder ShowWindow(ScrollDirection direction)
    {
        _commands.Add($"show:window:{direction.ToString().ToLowerInvariant()}");
        return this;
    }

    /// <summary>
    /// Builds the swap style command string with all specified modifiers.
    /// </summary>
    /// <returns>A tuple containing the SwapStyle and the constructed command string.</returns>
    internal (SwapStyle, string) Build()
    {
        return (_style, String.Join(' ', _commands));
    }
}

/// <summary>
/// Extension methods for the SwapStyle enum to facilitate building swap style commands.
/// </summary>
public static class SwapStyleBuilderExtension
{
    // Each method below returns a SwapStyleBuilder instance initialized with the respective SwapStyle
    // and applies the specified modifier to it. This allows for fluent configuration of swap style commands.

    public static SwapStyleBuilder After(this SwapStyle style, TimeSpan span) => new SwapStyleBuilder(style).After(span);
    public static SwapStyleBuilder Scroll(this SwapStyle style, ScrollDirection direction) => new SwapStyleBuilder(style).Scroll(direction);
    public static SwapStyleBuilder IgnoreTitle(this SwapStyle style, bool ignore) => new SwapStyleBuilder(style).IgnoreTitle(ignore);
    public static SwapStyleBuilder Transition(this SwapStyle style, bool show) => new SwapStyleBuilder(style).Transition(show);
    public static SwapStyleBuilder FocusScroll(this SwapStyle style, bool scroll) => new SwapStyleBuilder(style).FocusScroll(scroll);
    public static SwapStyleBuilder ShowOn(this SwapStyle style, string selector, ScrollDirection direction) => new SwapStyleBuilder(style).ShowOn(selector, direction);
    public static SwapStyleBuilder ShowWindow(this SwapStyle style, ScrollDirection direction) => new SwapStyleBuilder(style).ShowWindow(direction);
}