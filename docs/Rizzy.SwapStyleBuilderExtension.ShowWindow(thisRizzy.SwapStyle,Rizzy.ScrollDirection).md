#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[SwapStyleBuilderExtension](Rizzy.SwapStyleBuilderExtension 'Rizzy.SwapStyleBuilderExtension')

## SwapStyleBuilderExtension.ShowWindow(this SwapStyle, ScrollDirection) Method

Specifies that the swap should show in the window by smoothly scrolling to either the top or bottom of the window.

```csharp
public static Rizzy.SwapStyleBuilder ShowWindow(this Rizzy.SwapStyle style, Rizzy.ScrollDirection direction);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilderExtension.ShowWindow(thisRizzy.SwapStyle,Rizzy.ScrollDirection).style'></a>

`style` [SwapStyle](Rizzy.SwapStyle 'Rizzy.SwapStyle')

The swap style.

<a name='Rizzy.SwapStyleBuilderExtension.ShowWindow(thisRizzy.SwapStyle,Rizzy.ScrollDirection).direction'></a>

`direction` [ScrollDirection](Rizzy.ScrollDirection 'Rizzy.ScrollDirection')

The direction to scroll the window after the swap.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder')  
A [SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
This method adds the modifier `show:window:`, directing the swap to display the specified  
element at the bottom of the window.