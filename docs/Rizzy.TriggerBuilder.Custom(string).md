#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[TriggerBuilder](Rizzy.TriggerBuilder.md 'Rizzy.TriggerBuilder')

## TriggerBuilder.Custom(string) Method

Specifies a custom trigger that will be included without changes `hx-trigger=""`.

```csharp
public Rizzy.TriggerBuilder Custom(string triggerDefinition);
```
#### Parameters

<a name='Rizzy.TriggerBuilder.Custom(string).triggerDefinition'></a>

`triggerDefinition` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The custom trigger definition string.

#### Returns
[TriggerBuilder](Rizzy.TriggerBuilder.md 'Rizzy.TriggerBuilder')  
This [TriggerBuilder](Rizzy.TriggerBuilder.md 'Rizzy.TriggerBuilder') instance.

### Example
  
```csharp  
Trigger.Custom("custom-event delay:2s")  
// Resulting hx-trigger: <div hx-get="/custom" hx-trigger="custom-event delay:2s">Custom Event</div>  
```

### Remarks
This method sets a custom trigger definition.