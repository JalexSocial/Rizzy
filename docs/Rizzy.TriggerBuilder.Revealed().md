#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[TriggerBuilder](Rizzy.TriggerBuilder 'Rizzy.TriggerBuilder')

## TriggerBuilder.Revealed() Method

Specifies that the trigger occurs when an element is scrolled into the viewport by setting the event name `hx-trigger="revealed"`.

```csharp
public Rizzy.TriggerModifierBuilder Revealed();
```

#### Returns
[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')  
A [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder') instance to allow further configuration of the trigger.

### Example
  
```csharp  
Trigger.Revealed()  
// Resulting hx-trigger: <div hx-get="/load" hx-trigger="revealed">Reveal Me</div>  
```

### Remarks
This method sets the revealed event trigger for an AJAX request, useful for lazy-loading content as it enters the viewport.