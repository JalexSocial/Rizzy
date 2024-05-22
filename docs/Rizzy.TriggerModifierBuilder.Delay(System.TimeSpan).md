#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')

## TriggerModifierBuilder.Delay(TimeSpan) Method

Adds a delay before the event triggers the request by adding the modifier `delay:`.

```csharp
public Rizzy.TriggerModifierBuilder Delay(System.TimeSpan timing);
```
#### Parameters

<a name='Rizzy.TriggerModifierBuilder.Delay(System.TimeSpan).timing'></a>

`timing` [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan')

The delay time as a [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan').

#### Returns
[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')  
This [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder') instance.

### Example
  
```csharp  
Trigger.OnEvent("keyup").Delay(TimeSpan.FromSeconds(1))  
// Resulting hx-trigger: <div hx-get="/search" hx-trigger="keyup delay:1s">Search Me  
```

### Remarks
This method adds a delay to the trigger, resetting the delay if the event occurs again before the delay completes.