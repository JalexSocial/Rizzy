#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[TriggerBuilder](Rizzy.TriggerBuilder 'Rizzy.TriggerBuilder')

## TriggerBuilder.OnEvent(string) Method

Specifies a standard event as the trigger by setting the event name `hx-trigger=""`.

```csharp
public Rizzy.TriggerModifierBuilder OnEvent(string eventName);
```
#### Parameters

<a name='Rizzy.TriggerBuilder.OnEvent(string).eventName'></a>

`eventName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The name of the standard event (e.g., "click").

#### Returns
[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')  
A [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder') instance to allow further configuration of the trigger.

### Example
  
```csharp  
Trigger.OnEvent("click")  
// Resulting hx-trigger: <div hx-get="/clicked" hx-trigger="click">Click Me</div>  
```

### Remarks
This method builds a javascript event trigger. For example, specifying "click" will trigger the request on a click event.