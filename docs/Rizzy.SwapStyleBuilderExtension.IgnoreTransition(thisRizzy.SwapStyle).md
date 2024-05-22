#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[SwapStyleBuilderExtension](Rizzy.SwapStyleBuilderExtension 'Rizzy.SwapStyleBuilderExtension')

## SwapStyleBuilderExtension.IgnoreTransition(this SwapStyle) Method

Explicitly ignores transition effects for the swap.

```csharp
public static Rizzy.SwapStyleBuilder IgnoreTransition(this Rizzy.SwapStyle style);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilderExtension.IgnoreTransition(thisRizzy.SwapStyle).style'></a>

`style` [SwapStyle](Rizzy.SwapStyle 'Rizzy.SwapStyle')

The swap style.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder')  
A [SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
This method disables transition effects by adding the modifier `transition:false` to the swap commands.