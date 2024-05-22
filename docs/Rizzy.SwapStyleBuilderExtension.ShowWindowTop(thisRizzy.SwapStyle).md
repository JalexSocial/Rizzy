#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[SwapStyleBuilderExtension](Rizzy.SwapStyleBuilderExtension 'Rizzy.SwapStyleBuilderExtension')

## SwapStyleBuilderExtension.ShowWindowTop(this SwapStyle) Method

Specifies that the swap should smoothly scroll to the top of the window.

```csharp
public static Rizzy.SwapStyleBuilder ShowWindowTop(this Rizzy.SwapStyle style);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilderExtension.ShowWindowTop(thisRizzy.SwapStyle).style'></a>

`style` [SwapStyle](Rizzy.SwapStyle 'Rizzy.SwapStyle')

The swap style.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder')  
A [SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
This method adds the modifier `show:window:top`, instructing the content to be displayed  
at the top of the window following a swap by smoothly animating the scrollbar position. This can be useful  
for ensuring that important content or notifications at the top of the page are immediately visible to  
the user after a swap operation.