#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[TriggerBuilder](Rizzy.TriggerBuilder.md 'Rizzy.TriggerBuilder')

## TriggerBuilder.Load() Method

Specifies that the trigger occurs on page load by setting the event name `hx-trigger="load"`.

```csharp
public Rizzy.TriggerModifierBuilder Load();
```

#### Returns
[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder.md 'Rizzy.TriggerModifierBuilder')  
A [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder.md 'Rizzy.TriggerModifierBuilder') instance to allow further configuration of the trigger.

### Example
  
```csharp  
Trigger.Load()  
// Resulting hx-trigger: <div hx-get="/load" hx-trigger="load">Load Me</div>  
```

### Remarks
This method sets the load event trigger, useful for lazy-loading content.