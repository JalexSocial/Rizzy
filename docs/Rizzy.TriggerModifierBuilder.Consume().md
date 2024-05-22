#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')

## TriggerModifierBuilder.Consume() Method

Specifies that the event should not trigger any other htmx requests on parent elements by adding the modifier `consume`.

```csharp
public Rizzy.TriggerModifierBuilder Consume();
```

#### Returns
[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')  
This [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder') instance.

### Example
  
```csharp  
Trigger.OnEvent("click").Consume()  
// Resulting hx-trigger: <div hx-get="/click" hx-trigger="click consume">Click Me  
```

### Remarks
This method prevents the event from triggering other htmx requests on parent elements.