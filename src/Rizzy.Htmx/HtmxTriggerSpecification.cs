using System;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;

namespace Rizzy.Htmx;

/// <summary>
/// Represents a single htmx trigger definition, including the event and any modifiers.
/// See: https://htmx.org/attributes/hx-trigger/
/// </summary>
/// <remarks>
/// This record allows for building complex htmx trigger specifications programmatically,
/// often using a fluent pattern. Properties use <c>set</c> accessors allowing modification
/// after initial construction.
/// The <see cref="ToString()"/> method generates the string representation expected by htmx.
/// </remarks>
public sealed record HtmxTriggerSpecification
{
    // Use Lazy<T> to ensure thread‑safe, one‑time calculation and caching of the string representation.
    // PublicationOnly mode is suitable here: if multiple threads race to initialize,
    // one wins, its result is cached and used by all subsequent calls.
    // This relies on the assumption that the object's state is stable before the first call to ToString().
    private readonly Lazy<string> _lazyToString;

    private static readonly string[] s_extendedSelectorKeywords =
        ["closest", "find", "next", "previous"];

    /// <summary>Gets or sets the main event that triggers the request (e.g., "click", "load", "revealed", "every").</summary>
    [JsonPropertyName("trigger")]
    public string Trigger { get; set; } = string.Empty;

    /// <summary>Gets or sets the name of the Server‑Sent Event (SSE) to trigger on. Modifier: sseEvent:&lt;event_name&gt;</summary>
    [JsonPropertyName("sseEvent")]
    public string? SseEvent { get; set; }

    /// <summary>Gets or sets a JavaScript expression to filter events (e.g., "event.key == 'Enter'").</summary>
    [JsonPropertyName("eventFilter")]
    public string? EventFilter { get; set; }

    /// <summary>Gets or sets whether the request should only be triggered if the value of the element has changed. Modifier: changed</summary>
    [JsonPropertyName("changed")]
    public bool? Changed { get; set; }

    /// <summary>Gets or sets whether the request should only be triggered once. Modifier: once</summary>
    [JsonPropertyName("once")]
    public bool? Once { get; set; }

    /// <summary>Gets or sets whether the event should be consumed after triggering, preventing propagation. Modifier: consume</summary>
    [JsonPropertyName("consume")]
    public bool? Consume { get; set; }

    /// <summary>Gets or sets an extended CSS selector specifying a source element for the event. Modifier: from:&lt;selector&gt;</summary>
    [JsonPropertyName("from")]
    public string? From { get; set; }

    /// <summary>Gets or sets a CSS selector specifying a target element for the event listener. Modifier: target:&lt;selector&gt;</summary>
    [JsonPropertyName("target")]
    public string? Target { get; set; }

    /// <summary>Gets or sets the throttle time in milliseconds. Modifier: throttle:&lt;time&gt;ms</summary>
    [JsonPropertyName("throttle")]
    public int? Throttle { get; set; }

    /// <summary>Gets or sets the queueing behavior ("first", "last", "all", or "none"). Modifier: queue:&lt;value&gt;</summary>
    [JsonPropertyName("queue")]
    public string? Queue { get; set; }

    /// <summary>Gets or sets a CSS selector defining the root element for intersection observers. Modifier: root:&lt;selector&gt;</summary>
    [JsonPropertyName("root")]
    public string? Root { get; set; }

    /// <summary>Gets or sets the intersection threshold for the "revealed" trigger. Modifier: threshold:&lt;value&gt;</summary>
    [JsonPropertyName("threshold")]
    public string? Threshold { get; set; }

    /// <summary>Gets or sets the delay time in milliseconds before the request is triggered. Modifier: delay:&lt;time&gt;ms</summary>
    [JsonPropertyName("delay")]
    public int? Delay { get; set; }

    /// <summary>Gets or sets the polling interval (ms) when Trigger is "every".</summary>
    [JsonPropertyName("pollInterval")]
    public int? PollInterval { get; set; }

    /// <summary>Initializes a new instance of the <see cref="HtmxTriggerSpecification"/> class.</summary>
    public HtmxTriggerSpecification()
    {
        _lazyToString = new Lazy<string>(BuildTriggerString, LazyThreadSafetyMode.PublicationOnly);
    }

    /// <inheritdoc/>
    public override string ToString() => _lazyToString.Value;

    /// <summary>Builds the trigger string once per instance.</summary>
    private string BuildTriggerString()
    {
        var sb = new StringBuilder(128);
        var needsSpace = false;

        void AppendSpaceIfNeeded()
        {
            if (needsSpace) sb.Append(' ');
            else needsSpace = true;
        }

        // --- Core trigger / filter --------------------------------------------------
        sb.Append(Trigger);

        if (Trigger == Constants.Triggers.Every)
        {
            sb.Append(' ')
              .Append(FormatTimeSpan(TimeSpan.FromMilliseconds(PollInterval ?? 0)));
        }

        if (!string.IsNullOrEmpty(EventFilter))
            sb.Append('[').Append(EventFilter).Append(']');

        needsSpace = true;               
        // ---------------------------------------------------------------------------

        // --- Modifiers -------------------------------------------------------------
        if (!string.IsNullOrEmpty(SseEvent))
        {
            AppendSpaceIfNeeded();
            sb.Append(Constants.TriggerModifiers.SseEvent).Append(':').Append(SseEvent);
        }

        if (Once == true)
        {
            AppendSpaceIfNeeded();
            sb.Append(Constants.TriggerModifiers.Once);
        }

        if (Changed == true)
        {
            AppendSpaceIfNeeded();
            sb.Append(Constants.TriggerModifiers.Changed);
        }

        if (Delay.HasValue)
        {
            AppendSpaceIfNeeded();
            sb.Append(Constants.TriggerModifiers.Delay).Append(':')
              .Append(FormatTimeSpan(TimeSpan.FromMilliseconds(Delay.Value)));
        }

        if (Throttle.HasValue)
        {
            AppendSpaceIfNeeded();
            sb.Append(Constants.TriggerModifiers.Throttle).Append(':')
              .Append(FormatTimeSpan(TimeSpan.FromMilliseconds(Throttle.Value)));
        }

        if (!string.IsNullOrEmpty(From))
        {
            AppendSpaceIfNeeded();
            sb.Append(Constants.TriggerModifiers.From).Append(':')
              .Append(FormatExtendedCssSelector(From));
        }

        if (!string.IsNullOrEmpty(Target))
        {
            AppendSpaceIfNeeded();
            sb.Append(Constants.TriggerModifiers.Target).Append(':')
              .Append(FormatCssSelector(Target));
        }

        if (Consume == true)
        {
            AppendSpaceIfNeeded();
            sb.Append(Constants.TriggerModifiers.Consume);
        }

        if (!string.IsNullOrEmpty(Queue))
        {
            AppendSpaceIfNeeded();
            sb.Append(Constants.TriggerModifiers.Queue).Append(':').Append(Queue);
        }

        if (!string.IsNullOrEmpty(Root))
        {
            AppendSpaceIfNeeded();
            sb.Append(Constants.TriggerModifiers.Root).Append(':')
              .Append(FormatCssSelector(Root));
        }

        if (!string.IsNullOrEmpty(Threshold))
        {
            AppendSpaceIfNeeded();
            sb.Append(Constants.TriggerModifiers.Threshold).Append(':').Append(Threshold);
        }

        return sb.ToString();
    }

    #region Helper Methods

    /// <summary>
    /// Formats a TimeSpan into the string format expected by htmx (e.g., "500ms", "2s").
    /// </summary>
    private static string FormatTimeSpan(TimeSpan timing)
        => timing.TotalSeconds < 1
           ? $"{timing.TotalMilliseconds}ms"
           : $"{timing.TotalSeconds}s";

    /// <summary>Formats a selector for 'from' that may start with keywords (closest/find/next/previous).</summary>
    private static string FormatExtendedCssSelector(string cssSelector)
    {
        cssSelector = cssSelector.TrimStart();

        foreach (var keyword in s_extendedSelectorKeywords)
        {
            if (cssSelector.StartsWith(keyword + " ", StringComparison.Ordinal))
            {
                var selector = cssSelector.Substring(keyword.Length + 1);
                return keyword + " " + FormatCssSelector(selector);
            }
        }

        return FormatCssSelector(cssSelector);
    }

    /// <summary>
    /// Formats a standard CSS selector, wrapping whitespace‑containing selectors in '{}'.
    /// </summary>
    private static string FormatCssSelector(string cssSelector)
    {
        cssSelector = cssSelector.Trim();

        if (cssSelector.Length >= 2 &&
            ((cssSelector[0] == '{' && cssSelector[^1] == '}') ||
             (cssSelector[0] == '(' && cssSelector[^1] == ')')))
        {
            return cssSelector;
        }

        foreach (var ch in cssSelector)
        {
            if (char.IsWhiteSpace(ch))
            {
                return $"{{{cssSelector}}}";
            }
        }

        return cssSelector;
    }

    #endregion // Helper Methods
}
