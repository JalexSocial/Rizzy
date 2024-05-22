#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')

## TriggerModifierBuilder.WithCondition(string) Method

Adds a condition to the trigger by setting an event filter `[]`.

```csharp
public Rizzy.TriggerModifierBuilder WithCondition(string condition);
```
#### Parameters

<a name='Rizzy.TriggerModifierBuilder.WithCondition(string).condition'></a>

`condition` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The JavaScript expression to evaluate.

#### Returns
[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')  
This [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder') instance.

### Example
  
```csharp  
Trigger.OnEvent("click").WithCondition("ctrlKey")  
// Resulting hx-trigger: <div hx-get="/clicked" hx-trigger="click[ctrlKey]">Control Click Me  
```

### Remarks
This method adds a JavaScript expression as a condition for the event to be triggered.