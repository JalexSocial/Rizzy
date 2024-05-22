#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy')

## Trigger Class

Creates builder objects to begin the fluent chain for constructing triggers

```csharp
public static class Trigger
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; Trigger

| Methods | |
| :--- | :--- |
| [Custom(string)](Rizzy.Trigger.Custom(string).md 'Rizzy.Trigger.Custom(string)') | Specifies a custom trigger that will be included without changes `hx-trigger=""`. |
| [Every(TimeSpan)](Rizzy.Trigger.Every(System.TimeSpan).md 'Rizzy.Trigger.Every(System.TimeSpan)') | Specifies that the trigger occurs at regular intervals by setting the event name `hx-trigger="every  "`. |
| [Intersect(string, Nullable&lt;float&gt;)](Rizzy.Trigger.Intersect(string,System.Nullable_float_).md 'Rizzy.Trigger.Intersect(string, System.Nullable<float>)') | Specifies that the trigger occurs when an element intersects the viewport by setting the event name `hx-trigger="intersect"`. |
| [Load()](Rizzy.Trigger.Load().md 'Rizzy.Trigger.Load()') | Specifies that the trigger occurs on page load by setting the event name `hx-trigger="load"`. |
| [OnEvent(string)](Rizzy.Trigger.OnEvent(string).md 'Rizzy.Trigger.OnEvent(string)') | Specifies a standard event as the trigger by setting the event name `hx-trigger=""`. |
| [Revealed()](Rizzy.Trigger.Revealed().md 'Rizzy.Trigger.Revealed()') | Specifies that the trigger occurs when an element is scrolled into the viewport by setting the event name `hx-trigger="revealed"`. |
| [Sse(string)](Rizzy.Trigger.Sse(string).md 'Rizzy.Trigger.Sse(string)') | Specifies a Server-Sent Event (SSE) as the trigger by setting the event name and SSE event `hx-trigger="sse: "`. |
