using System.Text;
using System.Text.Json.Serialization;

namespace Rizzy.Htmx;

/// <summary>
/// Represents a single htmx trigger definition, including the event and any modifiers.
/// See: https://four.htmx.org/attributes/hx-trigger/
/// </summary>
/// <remarks>
/// <para>
/// This record allows for building complex htmx trigger specifications programmatically,
/// often using a fluent pattern. Properties use <c>set</c> accessors allowing modification
/// after initial construction.
/// </para>
/// <para>
/// <see cref="ToString()"/> generates the htmx 4 syntax on every call, so later changes and
/// <c>with</c>-copies always render their current values. Selectors containing whitespace or
/// commas are quoted (<c>from:'closest form'</c>) and times are formatted with the invariant
/// culture (<c>delay:1500ms</c>).
/// </para>
/// </remarks>
public sealed record HtmxTriggerSpecification
{
    /// <summary>Gets or sets the main event that triggers the request (e.g., "click", "load", "revealed", "every").</summary>
    [JsonPropertyName("trigger")]
    public string Trigger { get; set; } = string.Empty;

    /// <summary>Gets or sets a JavaScript expression to filter events (e.g., "event.key == 'Enter'").</summary>
    [JsonPropertyName("eventFilter")]
    public string? EventFilter { get; set; }

    /// <summary>Gets or sets whether the request should only be triggered if the value of the element has changed. Modifier: changed</summary>
    [JsonPropertyName("changed")]
    public bool? Changed { get; set; }

    /// <summary>Gets or sets whether the request should only be triggered once. Modifier: once</summary>
    [JsonPropertyName("once")]
    public bool? Once { get; set; }

    /// <summary>Gets or sets whether the event should stop propagating after triggering. Modifier: consume</summary>
    [JsonPropertyName("consume")]
    public bool? Consume { get; set; }

    /// <summary>Gets or sets whether <c>preventDefault()</c> is called on the event. Modifier: prevent</summary>
    [JsonPropertyName("prevent")]
    public bool? Prevent { get; set; }

    /// <summary>Gets or sets whether <c>stopPropagation()</c> is called on the event. Modifier: stop</summary>
    [JsonPropertyName("stop")]
    public bool? Stop { get; set; }

    /// <summary>Gets or sets whether both <c>preventDefault()</c> and <c>stopPropagation()</c> are called. Modifier: halt</summary>
    [JsonPropertyName("halt")]
    public bool? Halt { get; set; }

    /// <summary>Gets or sets whether the listener is registered in the capture phase. Modifier: capture</summary>
    [JsonPropertyName("capture")]
    public bool? Capture { get; set; }

    /// <summary>Gets or sets whether the listener is registered as passive. Modifier: passive</summary>
    [JsonPropertyName("passive")]
    public bool? Passive { get; set; }

    /// <summary>
    /// Gets or sets an extended CSS selector specifying a source element for the event, or the
    /// keywords <c>self</c> / <c>outside</c>. Modifier: from:&lt;selector&gt;
    /// </summary>
    [JsonPropertyName("from")]
    public string? From { get; set; }

    /// <summary>Gets or sets a CSS selector the event target must match. Modifier: target:&lt;selector&gt;</summary>
    [JsonPropertyName("target")]
    public string? Target { get; set; }

    /// <summary>Gets or sets the throttle time in milliseconds. Modifier: throttle:&lt;time&gt;</summary>
    [JsonPropertyName("throttle")]
    public int? Throttle { get; set; }

    /// <summary>Gets or sets a CSS selector defining the root element for intersection observers. Modifier: root:&lt;selector&gt;</summary>
    [JsonPropertyName("root")]
    public string? Root { get; set; }

    /// <summary>Gets or sets the intersection observer root margin (e.g. "10px 0px"). Modifier: rootMargin:&lt;value&gt;</summary>
    [JsonPropertyName("rootMargin")]
    public string? RootMargin { get; set; }

    /// <summary>Gets or sets the intersection threshold for the "revealed" trigger. Modifier: threshold:&lt;value&gt;</summary>
    [JsonPropertyName("threshold")]
    public string? Threshold { get; set; }

    /// <summary>Gets or sets the delay time in milliseconds before the request is triggered. Modifier: delay:&lt;time&gt;</summary>
    [JsonPropertyName("delay")]
    public int? Delay { get; set; }

    /// <summary>Gets or sets the polling interval (ms) when Trigger is "every".</summary>
    [JsonPropertyName("pollInterval")]
    public int? PollInterval { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder(128);

        sb.Append(Trigger);

        if (Trigger == Constants.Triggers.Every)
            sb.Append(' ').Append(HconFormat.Interval(PollInterval ?? 0));

        if (!string.IsNullOrEmpty(EventFilter))
            sb.Append('[').Append(EventFilter).Append(']');

        AppendFlag(sb, Constants.TriggerModifiers.Once, Once);
        AppendFlag(sb, Constants.TriggerModifiers.Changed, Changed);

        if (Delay.HasValue)
            Append(sb, Constants.TriggerModifiers.Delay, HconFormat.Interval(Delay.Value));

        if (Throttle.HasValue)
            Append(sb, Constants.TriggerModifiers.Throttle, HconFormat.Interval(Throttle.Value));

        if (!string.IsNullOrWhiteSpace(From))
            Append(sb, Constants.TriggerModifiers.From, HconFormat.Value(From));

        if (!string.IsNullOrWhiteSpace(Target))
            Append(sb, Constants.TriggerModifiers.Target, HconFormat.Value(Target));

        AppendFlag(sb, Constants.TriggerModifiers.Consume, Consume);
        AppendFlag(sb, Constants.TriggerModifiers.Prevent, Prevent);
        AppendFlag(sb, Constants.TriggerModifiers.Stop, Stop);
        AppendFlag(sb, Constants.TriggerModifiers.Halt, Halt);
        AppendFlag(sb, Constants.TriggerModifiers.Capture, Capture);
        AppendFlag(sb, Constants.TriggerModifiers.Passive, Passive);

        if (!string.IsNullOrWhiteSpace(Root))
            Append(sb, Constants.TriggerModifiers.Root, HconFormat.Value(Root));

        if (!string.IsNullOrWhiteSpace(RootMargin))
            Append(sb, Constants.TriggerModifiers.RootMargin, HconFormat.Value(RootMargin));

        if (!string.IsNullOrWhiteSpace(Threshold))
            Append(sb, Constants.TriggerModifiers.Threshold, Threshold);

        return sb.ToString();
    }

    private static void AppendFlag(StringBuilder sb, string name, bool? value)
    {
        if (value == true)
            sb.Append(' ').Append(name);
    }

    private static void Append(StringBuilder sb, string name, string value)
        => sb.Append(' ').Append(name).Append(':').Append(value);
}
