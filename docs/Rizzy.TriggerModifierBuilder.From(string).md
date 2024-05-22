#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')

## TriggerModifierBuilder.From(string) Method

Specifies that the event should trigger from another element by adding the modifier `from:`.

```csharp
public Rizzy.TriggerModifierBuilder From(string extendedCssSelector);
```
#### Parameters

<a name='Rizzy.TriggerModifierBuilder.From(string).extendedCssSelector'></a>

`extendedCssSelector` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The extended CSS selector of the element to listen for events from.

#### Returns
[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')  
This [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder') instance.

### Example
  
```csharp  
Trigger.OnEvent("keydown").From("document")  
// Resulting hx-trigger: <div hx-get="/hotkeys" hx-trigger="keydown from:document">Listen on Document  
```

### Remarks
This method allows listening to events on a different element specified by the extended CSS selector.