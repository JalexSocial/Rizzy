#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')

## TriggerModifierBuilder.Throttle(TimeSpan) Method

Adds a throttle after the event triggers the request by adding the modifier `throttle:`.

```csharp
public Rizzy.TriggerModifierBuilder Throttle(System.TimeSpan timing);
```
#### Parameters

<a name='Rizzy.TriggerModifierBuilder.Throttle(System.TimeSpan).timing'></a>

`timing` [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan')

The throttle time as a [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan').

#### Returns
[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')  
This [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder') instance.

### Example
  
```csharp  
Trigger.OnEvent("click").Throttle(TimeSpan.FromSeconds(2))  
// Resulting hx-trigger: <div hx-get="/updates" hx-trigger="click throttle:2s">Click Me  
```

### Remarks
This method adds a throttle to the trigger, ignoring subsequent events until the throttle period completes.