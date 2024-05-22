#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')

## TriggerModifierBuilder.Once() Method

Specifies that the event should trigger only once by adding the modifier `once`.

```csharp
public Rizzy.TriggerModifierBuilder Once();
```

#### Returns
[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')  
This [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder') instance.

### Example
  
```csharp  
Trigger.OnEvent("click").Once()  
// Resulting hx-trigger: <div hx-get="/clicked" hx-trigger="click once">Click Me Once  
```

### Remarks
This method adds the "once" modifier to the trigger, making it fire only the first time the event occurs.