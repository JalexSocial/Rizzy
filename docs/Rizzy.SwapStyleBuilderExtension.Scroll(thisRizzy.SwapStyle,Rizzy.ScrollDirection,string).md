#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[SwapStyleBuilderExtension](Rizzy.SwapStyleBuilderExtension.md 'Rizzy.SwapStyleBuilderExtension')

## SwapStyleBuilderExtension.Scroll(this SwapStyle, ScrollDirection, string) Method

Specifies how to set the content scrollbar position after the swap and appends the modifier `scroll:`.

```csharp
public static Rizzy.SwapStyleBuilder Scroll(this Rizzy.SwapStyle style, Rizzy.ScrollDirection direction, string? cssSelector=null);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilderExtension.Scroll(thisRizzy.SwapStyle,Rizzy.ScrollDirection,string).style'></a>

`style` [SwapStyle](Rizzy.SwapStyle.md 'Rizzy.SwapStyle')

The swap style.

<a name='Rizzy.SwapStyleBuilderExtension.Scroll(thisRizzy.SwapStyle,Rizzy.ScrollDirection,string).direction'></a>

`direction` [ScrollDirection](Rizzy.ScrollDirection.md 'Rizzy.ScrollDirection')

The scroll direction after the swap.

<a name='Rizzy.SwapStyleBuilderExtension.Scroll(thisRizzy.SwapStyle,Rizzy.ScrollDirection,string).cssSelector'></a>

`cssSelector` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

Optional css selector of the target element.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')  
A [SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
Sets the swapped content scrollbar position after swapping immediately (without animation). For instance, using [Rizzy.ScrollDirection.top](https://docs.microsoft.com/en-us/dotnet/api/Rizzy.ScrollDirection.top 'Rizzy.ScrollDirection.top')  
will add the modifier `scroll:top` which sets the scrollbar position to the top of swap content after the swap.  
If css [cssSelector](Rizzy.SwapStyleBuilderExtension.Scroll(thisRizzy.SwapStyle,Rizzy.ScrollDirection,string).md#Rizzy.SwapStyleBuilderExtension.Scroll(thisRizzy.SwapStyle,Rizzy.ScrollDirection,string).cssSelector 'Rizzy.SwapStyleBuilderExtension.Scroll(this Rizzy.SwapStyle, Rizzy.ScrollDirection, string).cssSelector') is present then the page is scrolled to the [direction](Rizzy.SwapStyleBuilderExtension.Scroll(thisRizzy.SwapStyle,Rizzy.ScrollDirection,string).md#Rizzy.SwapStyleBuilderExtension.Scroll(thisRizzy.SwapStyle,Rizzy.ScrollDirection,string).direction 'Rizzy.SwapStyleBuilderExtension.Scroll(this Rizzy.SwapStyle, Rizzy.ScrollDirection, string).direction') of the content identified by the css cssSelector.