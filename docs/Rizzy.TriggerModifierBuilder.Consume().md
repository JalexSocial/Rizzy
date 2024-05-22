#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder.md 'Rizzy.TriggerModifierBuilder')

## TriggerModifierBuilder.Consume() Method

Specifies that the event should not trigger any other htmx requests on parent elements by adding the modifier `consume`.

```csharp
public Rizzy.TriggerModifierBuilder Consume();
```

#### Returns
[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder.md 'Rizzy.TriggerModifierBuilder')  
This [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder.md 'Rizzy.TriggerModifierBuilder') instance.

### Example
  
```csharp  
Trigger.OnEvent("click").Consume()  
// Resulting hx-trigger: <div hx-get="/click" hx-trigger="click consume">Click Me  
```

### Remarks
This method prevents the event from triggering other htmx requests on parent elements.