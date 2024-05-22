#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy')

## TriggerBuilder Class

Provides methods to construct and manage htmx trigger definitions for htmx requests.

```csharp
public sealed class TriggerBuilder
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; TriggerBuilder

### Example
  
```csharp  
var trigger = Trigger  
    .OnEvent("click")  
    .WithCondition("ctrlKey")  
    .Delay(TimeSpan.FromSeconds(1))  
    .Throttle(TimeSpan.FromSeconds(2))  
    .From("document")  
    .Target(".child")  
    .Consume()  
    .Queue(TriggerQueueOption.All);  
      
string triggerDefinition = trigger.ToString();  
// Resulting hx-trigger: "click[ctrlKey] delay:1s throttle:2s from:document target:.child consume queue:all"  
```

### Remarks
The [TriggerBuilder](Rizzy.TriggerBuilder.md 'Rizzy.TriggerBuilder') class offers a fluent API to specify various triggers for htmx requests.   
It allows the combination of standard events, server-sent events, and custom triggers, as well as modifiers   
to control the behavior of these triggers.

| Methods | |
| :--- | :--- |
| [Build()](Rizzy.TriggerBuilder.Build().md 'Rizzy.TriggerBuilder.Build()') | Builds a key value pair mapping the formatted trigger definition to the [HtmxTriggerSpecification](Rizzy.HtmxTriggerSpecification.md 'Rizzy.HtmxTriggerSpecification') |
| [Custom(string)](Rizzy.TriggerBuilder.Custom(string).md 'Rizzy.TriggerBuilder.Custom(string)') | Specifies a custom trigger that will be included without changes `hx-trigger=""`. |
| [Every(TimeSpan)](Rizzy.TriggerBuilder.Every(System.TimeSpan).md 'Rizzy.TriggerBuilder.Every(System.TimeSpan)') | Specifies that the trigger occurs at regular intervals by setting the event name `hx-trigger="every  "`. |
| [Intersect(string, Nullable&lt;float&gt;)](Rizzy.TriggerBuilder.Intersect(string,System.Nullable_float_).md 'Rizzy.TriggerBuilder.Intersect(string, System.Nullable<float>)') | Specifies that the trigger occurs when an element intersects the viewport by setting the event name `hx-trigger="intersect"`. |
| [Load()](Rizzy.TriggerBuilder.Load().md 'Rizzy.TriggerBuilder.Load()') | Specifies that the trigger occurs on page load by setting the event name `hx-trigger="load"`. |
| [OnEvent(string)](Rizzy.TriggerBuilder.OnEvent(string).md 'Rizzy.TriggerBuilder.OnEvent(string)') | Specifies a standard event as the trigger by setting the event name `hx-trigger=""`. |
| [Revealed()](Rizzy.TriggerBuilder.Revealed().md 'Rizzy.TriggerBuilder.Revealed()') | Specifies that the trigger occurs when an element is scrolled into the viewport by setting the event name `hx-trigger="revealed"`. |
| [Sse(string)](Rizzy.TriggerBuilder.Sse(string).md 'Rizzy.TriggerBuilder.Sse(string)') | Specifies a Server-Sent Event (SSE) as the trigger by setting the event name and SSE event `hx-trigger="sse: "`. |
| [ToString()](Rizzy.TriggerBuilder.ToString().md 'Rizzy.TriggerBuilder.ToString()') | Returns a properly formatted trigger definition that can be used as an hx-trigger value |

| Operators | |
| :--- | :--- |
| [implicit operator KeyValuePair&lt;string,IReadOnlyList&lt;HtmxTriggerSpecification&gt;&gt;(TriggerBuilder)](Rizzy.TriggerBuilder.op_ImplicitSystem.Collections.Generic.KeyValuePair_string,System.Collections.Generic.IReadOnlyList_Rizzy.HtmxTriggerSpecification__(Rizzy.TriggerBuilder).md 'Rizzy.TriggerBuilder.op_Implicit System.Collections.Generic.KeyValuePair<string,System.Collections.Generic.IReadOnlyList<Rizzy.HtmxTriggerSpecification>>(Rizzy.TriggerBuilder)') | Converts the builder to a key-value pair. |
