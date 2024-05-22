#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy')

## TriggerModifierBuilder Class

Provides methods to add modifiers to htmx trigger definitions within a [TriggerBuilder](Rizzy.TriggerBuilder 'Rizzy.TriggerBuilder') context.

```csharp
public sealed class TriggerModifierBuilder
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; TriggerModifierBuilder

### Remarks
The [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder') class extends the functionality of the [TriggerBuilder](Rizzy.TriggerBuilder 'Rizzy.TriggerBuilder')   
by allowing the addition of conditions, delays, throttles, source elements, target filters, consumption flags,   
and queuing options to the htmx trigger definitions.

| Methods | |
| :--- | :--- |
| [Build()](Rizzy.TriggerModifierBuilder.Build() 'Rizzy.TriggerModifierBuilder.Build()') | Converts the builder to a key-value pair. |
| [Changed()](Rizzy.TriggerModifierBuilder.Changed() 'Rizzy.TriggerModifierBuilder.Changed()') | Specifies that the event should trigger only when the value changes by adding the modifier `changed`. |
| [Consume()](Rizzy.TriggerModifierBuilder.Consume() 'Rizzy.TriggerModifierBuilder.Consume()') | Specifies that the event should not trigger any other htmx requests on parent elements by adding the modifier `consume`. |
| [Delay(TimeSpan)](Rizzy.TriggerModifierBuilder.Delay(System.TimeSpan) 'Rizzy.TriggerModifierBuilder.Delay(System.TimeSpan)') | Adds a delay before the event triggers the request by adding the modifier `delay:`. |
| [From(string)](Rizzy.TriggerModifierBuilder.From(string) 'Rizzy.TriggerModifierBuilder.From(string)') | Specifies that the event should trigger from another element by adding the modifier `from:`. |
| [Once()](Rizzy.TriggerModifierBuilder.Once() 'Rizzy.TriggerModifierBuilder.Once()') | Specifies that the event should trigger only once by adding the modifier `once`. |
| [Or()](Rizzy.TriggerModifierBuilder.Or() 'Rizzy.TriggerModifierBuilder.Or()') | Combines multiple triggers for a single element, allowing continuation of trigger configuration. |
| [Queue(TriggerQueueOption)](Rizzy.TriggerModifierBuilder.Queue(Rizzy.TriggerQueueOption) 'Rizzy.TriggerModifierBuilder.Queue(Rizzy.TriggerQueueOption)') | Specifies how events are queued when an event occurs while a request is in flight by adding the modifier `queue:`. |
| [Target(string)](Rizzy.TriggerModifierBuilder.Target(string) 'Rizzy.TriggerModifierBuilder.Target(string)') | Filters the event trigger to a specific target element by adding the modifier `target:`. |
| [Throttle(TimeSpan)](Rizzy.TriggerModifierBuilder.Throttle(System.TimeSpan) 'Rizzy.TriggerModifierBuilder.Throttle(System.TimeSpan)') | Adds a throttle after the event triggers the request by adding the modifier `throttle:`. |
| [ToString()](Rizzy.TriggerModifierBuilder.ToString() 'Rizzy.TriggerModifierBuilder.ToString()') | Returns a properly formatted trigger definition that can be used as an hx-trigger value |
| [WithCondition(string)](Rizzy.TriggerModifierBuilder.WithCondition(string) 'Rizzy.TriggerModifierBuilder.WithCondition(string)') | Adds a condition to the trigger by setting an event filter `[]`. |

| Operators | |
| :--- | :--- |
| [implicit operator KeyValuePair&lt;string,IReadOnlyList&lt;HtmxTriggerSpecification&gt;&gt;(TriggerModifierBuilder)](Rizzy.TriggerModifierBuilder.op_ImplicitSystem.Collections.Generic.KeyValuePair_string,System.Collections.Generic.IReadOnlyList_Rizzy.HtmxTriggerSpecification__(Rizzy.TriggerModifierBuilder) 'Rizzy.TriggerModifierBuilder.op_Implicit System.Collections.Generic.KeyValuePair<string,System.Collections.Generic.IReadOnlyList<Rizzy.HtmxTriggerSpecification>>(Rizzy.TriggerModifierBuilder)') | Converts the builder to a key-value pair. |
