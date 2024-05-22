#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder.md 'Rizzy.TriggerModifierBuilder')

## TriggerModifierBuilder.Or() Method

Combines multiple triggers for a single element, allowing continuation of trigger configuration.

```csharp
public Rizzy.TriggerBuilder Or();
```

#### Returns
[TriggerBuilder](Rizzy.TriggerBuilder.md 'Rizzy.TriggerBuilder')  
The parent [TriggerBuilder](Rizzy.TriggerBuilder.md 'Rizzy.TriggerBuilder') instance for additional configuration.

### Example
  
```csharp  
Trigger.OnEvent("click").Or().Load()  
// Resulting hx-trigger: <div hx-get="/news" hx-trigger="click, load">Click or Load  
```

### Remarks
This method is used to combine multiple triggers for a single element, returning the parent builder for further configuration.