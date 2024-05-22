#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[SwapStyleBuilderExtension](Rizzy.SwapStyleBuilderExtension 'Rizzy.SwapStyleBuilderExtension')

## SwapStyleBuilderExtension.Transition(this SwapStyle, bool) Method

Enables or disables transition effects for the swap by appending the modifier `transition:{show}`.

```csharp
public static Rizzy.SwapStyleBuilder Transition(this Rizzy.SwapStyle style, bool show);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilderExtension.Transition(thisRizzy.SwapStyle,bool).style'></a>

`style` [SwapStyle](Rizzy.SwapStyle 'Rizzy.SwapStyle')

The swap style.

<a name='Rizzy.SwapStyleBuilderExtension.Transition(thisRizzy.SwapStyle,bool).show'></a>

`show` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

Whether to show transition effects.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder')  
A [SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
Controls the display of transition effects during the swap. For example, setting [show](Rizzy.SwapStyleBuilderExtension.Transition(thisRizzy.SwapStyle,bool)#Rizzy.SwapStyleBuilderExtension.Transition(thisRizzy.SwapStyle,bool).show 'Rizzy.SwapStyleBuilderExtension.Transition(this Rizzy.SwapStyle, bool).show') to true  
will add the modifier `transition:true` to enable smooth transitions.