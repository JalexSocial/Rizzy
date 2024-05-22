#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder.md 'Rizzy.TriggerModifierBuilder')

## TriggerModifierBuilder.Changed() Method

Specifies that the event should trigger only when the value changes by adding the modifier `changed`.

```csharp
public Rizzy.TriggerModifierBuilder Changed();
```

#### Returns
[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder.md 'Rizzy.TriggerModifierBuilder')  
This [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder.md 'Rizzy.TriggerModifierBuilder') instance.

### Example
  
```csharp  
Trigger.OnEvent("keyup").Changed().Delay(TimeSpan.FromSeconds(1))  
// Resulting hx-trigger: <input hx-get="/search" hx-trigger="keyup changed delay:1s"/>  
```

### Remarks
This method adds the "changed" modifier to the trigger, making it fire only when the value of the element has changed.