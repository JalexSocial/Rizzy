#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')

## TriggerModifierBuilder.Queue(TriggerQueueOption) Method

Specifies how events are queued when an event occurs while a request is in flight by adding the modifier `queue:`.

```csharp
public Rizzy.TriggerModifierBuilder Queue(Rizzy.TriggerQueueOption option=Rizzy.TriggerQueueOption.last);
```
#### Parameters

<a name='Rizzy.TriggerModifierBuilder.Queue(Rizzy.TriggerQueueOption).option'></a>

`option` [TriggerQueueOption](Rizzy.TriggerQueueOption 'Rizzy.TriggerQueueOption')

(optional) The queue option as a [TriggerQueueOption](Rizzy.TriggerQueueOption 'Rizzy.TriggerQueueOption').

#### Returns
[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')  
This [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder') instance.

### Example
  
```csharp  
Trigger.OnEvent("click").Queue(TriggerQueueOption.All)  
// Resulting hx-trigger: <div hx-get="/process" hx-trigger="click queue:all">Queue All  
```

### Remarks
This method sets the event queuing option, such as "first", "last", "all", or "none".