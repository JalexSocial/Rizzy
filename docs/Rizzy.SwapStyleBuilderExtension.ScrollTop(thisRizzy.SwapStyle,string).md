#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[SwapStyleBuilderExtension](Rizzy.SwapStyleBuilderExtension.md 'Rizzy.SwapStyleBuilderExtension')

## SwapStyleBuilderExtension.ScrollTop(this SwapStyle, string) Method

Sets the content scrollbar position to the top of the swapped content after a swap.

```csharp
public static Rizzy.SwapStyleBuilder ScrollTop(this Rizzy.SwapStyle style, string? cssSelector=null);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilderExtension.ScrollTop(thisRizzy.SwapStyle,string).style'></a>

`style` [SwapStyle](Rizzy.SwapStyle.md 'Rizzy.SwapStyle')

The swap style.

<a name='Rizzy.SwapStyleBuilderExtension.ScrollTop(thisRizzy.SwapStyle,string).cssSelector'></a>

`cssSelector` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

Optional css selector of the target element.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')  
A [SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
This method adds the modifier `scroll:top` to the swap commands, instructing the page to scroll to  
the top of the content after content is swapped immediately and without animation. If css [cssSelector](Rizzy.SwapStyleBuilderExtension.ScrollTop(thisRizzy.SwapStyle,string).md#Rizzy.SwapStyleBuilderExtension.ScrollTop(thisRizzy.SwapStyle,string).cssSelector 'Rizzy.SwapStyleBuilderExtension.ScrollTop(this Rizzy.SwapStyle, string).cssSelector')  
is present then the page is scrolled to the top of the content identified by the css cssSelector.