using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Rizzy.Htmx;

/// <summary>
/// Provides methods to add modifiers to htmx trigger definitions within a <see cref="TriggerBuilder"/> context.
/// </summary>
/// <remarks>
/// The <see cref="TriggerModifierBuilder"/> class extends the functionality of the <see cref="TriggerBuilder"/> 
/// by allowing the addition of conditions, delays, throttles, source elements, target filters, consumption flags, 
/// and event-handling flags to the htmx trigger definitions.
/// </remarks>
[DebuggerDisplay("{ToString(),nq}")]
public sealed class TriggerModifierBuilder
{
    private readonly HtmxTriggerSpecification specification;
    private readonly TriggerBuilder parentBuilder;

    internal TriggerModifierBuilder(HtmxTriggerSpecification specification, TriggerBuilder parentBuilder)
    {
        this.specification = specification;
        this.parentBuilder = parentBuilder;

        parentBuilder.AddTrigger(specification);
    }

    /// <summary>
    /// Adds a condition to the trigger by setting an event filter <c>[<paramref name="condition"/>]</c>.
    /// </summary>
    /// <remarks>
    /// This method adds a JavaScript expression as a condition for the event to be triggered.
    /// </remarks>
    /// <param name="condition">The JavaScript expression to evaluate.</param>
    /// <returns>This <see cref="TriggerModifierBuilder"/> instance.</returns>
    /// <example>
    /// <code>
    /// Trigger.OnEvent("click").WithCondition("ctrlKey")
    /// // Resulting hx-trigger: <![CDATA[<div hx-get="/clicked" hx-trigger="click[ctrlKey]">Control Click Me]]>
    /// </code>
    /// </example>
    public TriggerModifierBuilder WithCondition(string condition)
    {
        specification.EventFilter = condition;
        return this;
    }

    /// <summary>
    /// Specifies that the event should trigger only once by adding the modifier <c>once</c>.
    /// </summary>
    /// <remarks>
    /// This method adds the "once" modifier to the trigger, making it fire only the first time the event occurs.
    /// </remarks>
    /// <returns>This <see cref="TriggerModifierBuilder"/> instance.</returns>
    /// <example>
    /// <code>
    /// Trigger.OnEvent("click").Once()
    /// // Resulting hx-trigger: <![CDATA[<div hx-get="/clicked" hx-trigger="click once">Click Me Once]]>
    /// </code>
    /// </example>
    public TriggerModifierBuilder Once()
    {
        specification.Once = true;
        return this;
    }

    /// <summary>
    /// Specifies that the event should trigger only when the value changes by adding the modifier <c>changed</c>.
    /// </summary>
    /// <remarks>
    /// This method adds the "changed" modifier to the trigger, making it fire only when the value of the element has changed.
    /// </remarks>
    /// <returns>This <see cref="TriggerModifierBuilder"/> instance.</returns>
    /// <example>
    /// <code>
    /// Trigger.OnEvent("keyup").Changed().Delay(TimeSpan.FromSeconds(1))
    /// // Resulting hx-trigger: <![CDATA[<input hx-get="/search" hx-trigger="keyup changed delay:1s"/>]]>
    /// </code>
    /// </example>
    public TriggerModifierBuilder Changed()
    {
        specification.Changed = true;
        return this;
    }

    /// <summary>
    /// Adds a delay before the event triggers the request by adding the modifier <c>delay:<paramref name="timing"/></c>.
    /// </summary>
    /// <remarks>
    /// This method adds a delay to the trigger, resetting the delay if the event occurs again before the delay completes.
    /// </remarks>
    /// <param name="timing">The delay time as a <see cref="TimeSpan"/>.</param>
    /// <returns>This <see cref="TriggerModifierBuilder"/> instance.</returns>
    /// <example>
    /// <code>
    /// Trigger.OnEvent("keyup").Delay(TimeSpan.FromSeconds(1))
    /// // Resulting hx-trigger: <![CDATA[<div hx-get="/search" hx-trigger="keyup delay:1s">Search Me]]>
    /// </code>
    /// </example>
    public TriggerModifierBuilder Delay(TimeSpan timing)
    {
        specification.Delay = (int)timing.TotalMilliseconds;
        return this;
    }

    /// <summary>
    /// Adds a throttle after the event triggers the request by adding the modifier <c>throttle:<paramref name="timing"/></c>.
    /// </summary>
    /// <remarks>
    /// This method adds a throttle to the trigger, ignoring subsequent events until the throttle period completes.
    /// </remarks>
    /// <param name="timing">The throttle time as a <see cref="TimeSpan"/>.</param>
    /// <returns>This <see cref="TriggerModifierBuilder"/> instance.</returns>
    /// <example>
    /// <code>
    /// Trigger.OnEvent("click").Throttle(TimeSpan.FromSeconds(2))
    /// // Resulting hx-trigger: <![CDATA[<div hx-get="/updates" hx-trigger="click throttle:2s">Click Me]]>
    /// </code>
    /// </example>
    public TriggerModifierBuilder Throttle(TimeSpan timing)
    {
        specification.Throttle = (int)timing.TotalMilliseconds;
        return this;
    }

    /// <summary>
    /// Specifies that the event should trigger from another element by adding the modifier <c>from:<paramref name="extendedCssSelector"/></c>.
    /// </summary>
    /// <remarks>
    /// This method allows listening to events on a different element specified by the extended CSS selector.
    /// </remarks>
    /// <param name="extendedCssSelector">The extended CSS selector of the element to listen for events from.</param>
    /// <returns>This <see cref="TriggerModifierBuilder"/> instance.</returns>
    /// <example>
    /// <code>
    /// Trigger.OnEvent("keydown").From("document")
    /// // Resulting hx-trigger: <![CDATA[<div hx-get="/hotkeys" hx-trigger="keydown from:document">Listen on Document]]>
    /// Trigger.OnEvent("submit").From("closest form")
    /// // Resulting hx-trigger: submit from:'closest form'
    /// </code>
    /// </example>
    public TriggerModifierBuilder From(string extendedCssSelector)
    {
        specification.From = extendedCssSelector;
        return this;
    }

    /// <summary>
    /// Filters the event trigger to a specific target element by adding the modifier <c>target:<paramref name="cssSelector"/></c>.
    /// </summary>
    /// <remarks>
    /// This method allows the event trigger to be filtered to a target element specified by the CSS selector.
    /// </remarks>
    /// <param name="cssSelector">The CSS selector of the target element.</param>
    /// <returns>This <see cref="TriggerModifierBuilder"/> instance.</returns>
    /// <example>
    /// <code>
    /// Trigger.OnEvent("click").Target(".child")
    /// // Resulting hx-trigger: <![CDATA[<div hx-get="/dynamic" hx-trigger="click target:.child">Click Child]]>
    /// </code>
    /// </example>
    public TriggerModifierBuilder Target(string cssSelector)
    {
        specification.Target = cssSelector;
        return this;
    }

    /// <summary>
    /// Specifies that the event should not trigger any other htmx requests on parent elements by adding the modifier <c>consume</c>.
    /// </summary>
    /// <remarks>
    /// This method prevents the event from triggering other htmx requests on parent elements.
    /// </remarks>
    /// <returns>This <see cref="TriggerModifierBuilder"/> instance.</returns>
    /// <example>
    /// <code>
    /// Trigger.OnEvent("click").Consume()
    /// // Resulting hx-trigger: <![CDATA[<div hx-get="/click" hx-trigger="click consume">Click Me]]>
    /// </code>
    /// </example>
    public TriggerModifierBuilder Consume()
    {
        specification.Consume = true;
        return this;
    }

    /// <summary>
    /// Calls <c>preventDefault()</c> on the triggering event by adding the modifier <c>prevent</c>.
    /// </summary>
    /// <returns>This <see cref="TriggerModifierBuilder"/> instance.</returns>
    /// <example>
    /// <code>
    /// Trigger.OnEvent("submit").Prevent()
    /// // Resulting hx-trigger: submit prevent
    /// </code>
    /// </example>
    public TriggerModifierBuilder Prevent()
    {
        specification.Prevent = true;
        return this;
    }

    /// <summary>
    /// Calls <c>stopPropagation()</c> on the triggering event by adding the modifier <c>stop</c>.
    /// </summary>
    /// <returns>This <see cref="TriggerModifierBuilder"/> instance.</returns>
    public TriggerModifierBuilder Stop()
    {
        specification.Stop = true;
        return this;
    }

    /// <summary>
    /// Calls both <c>preventDefault()</c> and <c>stopPropagation()</c> on the triggering event by
    /// adding the modifier <c>halt</c>.
    /// </summary>
    /// <returns>This <see cref="TriggerModifierBuilder"/> instance.</returns>
    public TriggerModifierBuilder Halt()
    {
        specification.Halt = true;
        return this;
    }

    /// <summary>
    /// Registers the event listener in the capture phase by adding the modifier <c>capture</c>.
    /// </summary>
    /// <returns>This <see cref="TriggerModifierBuilder"/> instance.</returns>
    public TriggerModifierBuilder Capture()
    {
        specification.Capture = true;
        return this;
    }

    /// <summary>
    /// Registers the event listener as passive by adding the modifier <c>passive</c>.
    /// </summary>
    /// <returns>This <see cref="TriggerModifierBuilder"/> instance.</returns>
    public TriggerModifierBuilder Passive()
    {
        specification.Passive = true;
        return this;
    }

    /// <summary>
    /// Only triggers when the event originates from the element itself, not a descendant, by
    /// adding the modifier <c>from:self</c>.
    /// </summary>
    /// <returns>This <see cref="TriggerModifierBuilder"/> instance.</returns>
    public TriggerModifierBuilder FromSelf()
    {
        specification.From = Constants.TriggerModifiers.FromSelf;
        return this;
    }

    /// <summary>
    /// Triggers when the event happens outside the element (for example, click-away) by adding the
    /// modifier <c>from:outside</c>.
    /// </summary>
    /// <returns>This <see cref="TriggerModifierBuilder"/> instance.</returns>
    /// <example>
    /// <code>
    /// Trigger.OnEvent("click").FromOutside()
    /// // Resulting hx-trigger: click from:outside
    /// </code>
    /// </example>
    public TriggerModifierBuilder FromOutside()
    {
        specification.From = Constants.TriggerModifiers.FromOutside;
        return this;
    }

    /// <summary>
    /// Sets the intersection observer root margin for <c>intersect</c>/<c>revealed</c> triggers by
    /// adding the modifier <c>rootMargin:<paramref name="margin"/></c>.
    /// </summary>
    /// <param name="margin">A CSS margin value such as <c>"10px 0px"</c>.</param>
    /// <returns>This <see cref="TriggerModifierBuilder"/> instance.</returns>
    public TriggerModifierBuilder RootMargin(string margin)
    {
        specification.RootMargin = margin;
        return this;
    }

    /// <summary>
    /// Combines multiple triggers for a single element, allowing continuation of trigger configuration.
    /// </summary>
    /// <remarks>
    /// This method is used to combine multiple triggers for a single element, returning the parent builder for further configuration.
    /// </remarks>
    /// <returns>The parent <see cref="TriggerBuilder"/> instance for additional configuration.</returns>
    /// <example>
    /// <code>
    /// Trigger.OnEvent("click").Or().Load()
    /// // Resulting hx-trigger: <![CDATA[<div hx-get="/news" hx-trigger="click, load">Click or Load]]>
    /// </code>
    /// </example>
    public TriggerBuilder Or()
    {
        return parentBuilder;
    }

    /// <summary>
    /// Converts the builder to a key-value pair.
    /// </summary>
    public KeyValuePair<string, IReadOnlyList<HtmxTriggerSpecification>> Build() => parentBuilder.Build();

    /// <summary>
    /// Returns a properly formatted trigger definition that can be used as an hx-trigger value
    /// </summary>
    /// <returns>trigger definition</returns>
    public override string ToString() => parentBuilder.ToString();

    /// <summary>
    /// Converts the builder to a key-value pair.
    /// </summary>
    [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "See Build() method.")]
    public static implicit operator KeyValuePair<string, IReadOnlyList<HtmxTriggerSpecification>>(TriggerModifierBuilder builders)
    {
        ArgumentNullException.ThrowIfNull(builders);
        return builders.Build();
    }
}

