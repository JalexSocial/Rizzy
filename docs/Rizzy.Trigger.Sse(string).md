#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[Trigger](Rizzy.Trigger 'Rizzy.Trigger')

## Trigger.Sse(string) Method

Specifies a Server-Sent Event (SSE) as the trigger by setting the event name and SSE event `hx-trigger="sse: "`.

```csharp
public static Rizzy.TriggerModifierBuilder Sse(string sseEventName);
```
#### Parameters

<a name='Rizzy.Trigger.Sse(string).sseEventName'></a>

`sseEventName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The name of the SSE event.

#### Returns
[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')  
A [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder') instance to allow further configuration of the trigger.

### Example
  
```csharp  
Trigger.Sse("message")  
// Resulting hx-trigger: <div hx-get="/updates" hx-trigger="sse:message">Update Me</div>  
```

### Remarks
This method sets the SSE trigger for an AJAX request. For example, specifying "message" will trigger the request on the message event.