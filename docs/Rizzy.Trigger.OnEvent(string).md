#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[Trigger](Rizzy.Trigger.md 'Rizzy.Trigger')

## Trigger.OnEvent(string) Method

Specifies a standard event as the trigger by setting the event name `hx-trigger=""`.

```csharp
public static Rizzy.TriggerModifierBuilder OnEvent(string eventName);
```
#### Parameters

<a name='Rizzy.Trigger.OnEvent(string).eventName'></a>

`eventName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The name of the standard event (e.g., "click").

#### Returns
[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder.md 'Rizzy.TriggerModifierBuilder')  
A [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder.md 'Rizzy.TriggerModifierBuilder') instance to allow further configuration of the trigger.

### Example
  
```csharp  
Trigger.OnEvent("click")  
// Resulting hx-trigger: <div hx-get="/clicked" hx-trigger="click">Click Me</div>  
```

### Remarks
This method builds a javascript event trigger. For example, specifying "click" will trigger the request on a click event.