#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[Trigger](Rizzy.Trigger.md 'Rizzy.Trigger')

## Trigger.Revealed() Method

Specifies that the trigger occurs when an element is scrolled into the viewport by setting the event name `hx-trigger="revealed"`.

```csharp
public static Rizzy.TriggerModifierBuilder Revealed();
```

#### Returns
[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder.md 'Rizzy.TriggerModifierBuilder')  
A [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder.md 'Rizzy.TriggerModifierBuilder') instance to allow further configuration of the trigger.

### Example
  
```csharp  
Trigger.Revealed()  
// Resulting hx-trigger: <div hx-get="/load" hx-trigger="revealed">Reveal Me</div>  
```

### Remarks
This method sets the revealed event trigger for an AJAX request, useful for lazy-loading content as it enters the viewport.