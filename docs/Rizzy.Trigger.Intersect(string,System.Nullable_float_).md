#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[Trigger](Rizzy.Trigger 'Rizzy.Trigger')

## Trigger.Intersect(string, Nullable<float>) Method

Specifies that the trigger occurs when an element intersects the viewport by setting the event name `hx-trigger="intersect"`.

```csharp
public static Rizzy.TriggerModifierBuilder Intersect(string? root=null, System.Nullable<float> threshold=null);
```
#### Parameters

<a name='Rizzy.Trigger.Intersect(string,System.Nullable_float_).root'></a>

`root` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

(optional) The CSS selector of the root element for intersection.

<a name='Rizzy.Trigger.Intersect(string,System.Nullable_float_).threshold'></a>

`threshold` [System.Nullable&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1 'System.Nullable`1')[System.Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single 'System.Single')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1 'System.Nullable`1')

(optional) A floating point number between 0.0 and 1.0 indicating what amount of intersection to fire the event on.

#### Returns
[TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder')  
A [TriggerModifierBuilder](Rizzy.TriggerModifierBuilder 'Rizzy.TriggerModifierBuilder') instance to allow further configuration of the trigger.

### Example
  
```csharp  
Trigger.Intersect(root: ".container", threshold: 0.5f)  
// Resulting hx-trigger: <div hx-get="/load" hx-trigger="intersect root:.container threshold:0.5">Intersect Me</div>  
```

### Remarks
This method sets the intersect event trigger for an AJAX request, which fires when the element first intersects the viewport. Additional options include `root:` and `threshold:`.