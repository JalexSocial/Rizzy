#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[SwapStyleBuilderExtension](Rizzy.SwapStyleBuilderExtension.md 'Rizzy.SwapStyleBuilderExtension')

## SwapStyleBuilderExtension.ShowWindowBottom(this SwapStyle) Method

Specifies that the swap should smoothly scroll to the bottom of the window.

```csharp
public static Rizzy.SwapStyleBuilder ShowWindowBottom(this Rizzy.SwapStyle style);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilderExtension.ShowWindowBottom(thisRizzy.SwapStyle).style'></a>

`style` [SwapStyle](Rizzy.SwapStyle.md 'Rizzy.SwapStyle')

The swap style.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')  
A [SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
This method adds the modifier `show:window:bottom`, instructing the content to be displayed  
at the bottom of the window following a swap by smoothly animating the scrollbar position. This positioning  
can be used for infinite scrolling, footers, or information appended at the end of the page.