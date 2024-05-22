#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')

## TriggerModifierBuilder.Target(string) Method

Filters the event trigger to a specific target element by adding the modifier `target:`.

```csharp
public Rizzy.TriggerModifierBuilder Target(string cssSelector);
```
#### Parameters

<a name='Rizzy.TriggerModifierBuilder.Target(string).cssSelector'></a>

`cssSelector` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The CSS selector of the target element.

#### Returns
[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')  
This [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder') instance.

### Example
  
```csharp  
Trigger.OnEvent("click").Target(".child")  
// Resulting hx-trigger: <div hx-get="/dynamic" hx-trigger="click target:.child">Click Child  
```

### Remarks
This method allows the event trigger to be filtered to a target element specified by the CSS selector.