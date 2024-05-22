#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[TriggerBuilder](Rizzy.TriggerBuilder 'Rizzy.TriggerBuilder')

## TriggerBuilder.Every(TimeSpan) Method

Specifies that the trigger occurs at regular intervals by setting the event name `hx-trigger="every  "`.

```csharp
public Rizzy.TriggerModifierBuilder Every(System.TimeSpan interval);
```
#### Parameters

<a name='Rizzy.TriggerBuilder.Every(System.TimeSpan).interval'></a>

`interval` [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan')

The interval at which to poll, as a [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan').

#### Returns
[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')  
A [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder') instance to allow further configuration of the trigger.

### Example
  
```csharp  
Trigger.Every(TimeSpan.FromSeconds(5))  
// Resulting hx-trigger: <div hx-get="/updates" hx-trigger="every 5s">Update Every 5s</div>  
```

### Remarks
This method sets the polling interval for an AJAX request. For example, specifying an interval of 1 second will trigger the request every second.