#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')

## SwapStyleBuilder.Transition(bool) Method

Enables or disables transition effects for the swap by appending the modifier `transition:`.

```csharp
public Rizzy.SwapStyleBuilder Transition(bool show);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilder.Transition(bool).show'></a>

`show` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

Whether to show transition effects.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')  
This [SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
Controls the display of transition effects during the swap. For example, setting [show](Rizzy.SwapStyleBuilder.Transition(bool).md#Rizzy.SwapStyleBuilder.Transition(bool).show 'Rizzy.SwapStyleBuilder.Transition(bool).show') to true  
will add the modifier `transition:true` to enable smooth transitions.