#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')

## SwapStyleBuilder.Scroll(ScrollDirection, string) Method

Specifies how to set the content scrollbar position after the swap and appends the modifier `scroll:`.

```csharp
public Rizzy.SwapStyleBuilder Scroll(Rizzy.ScrollDirection direction, string? cssSelector=null);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilder.Scroll(Rizzy.ScrollDirection,string).direction'></a>

`direction` [ScrollDirection](Rizzy.ScrollDirection.md 'Rizzy.ScrollDirection')

The scroll direction after the swap.

<a name='Rizzy.SwapStyleBuilder.Scroll(Rizzy.ScrollDirection,string).cssSelector'></a>

`cssSelector` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

Optional CSS cssSelector of the target element.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')  
This [SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
Sets the swapped content scrollbar position after swapping immediately (without animation). For instance, using [Rizzy.ScrollDirection.top](https://docs.microsoft.com/en-us/dotnet/api/Rizzy.ScrollDirection.top 'Rizzy.ScrollDirection.top')  
will add the modifier `scroll:top` which sets the scrollbar position to the top of swap content after the swap.  
If css [cssSelector](Rizzy.SwapStyleBuilder.Scroll(Rizzy.ScrollDirection,string).md#Rizzy.SwapStyleBuilder.Scroll(Rizzy.ScrollDirection,string).cssSelector 'Rizzy.SwapStyleBuilder.Scroll(Rizzy.ScrollDirection, string).cssSelector') is present then the page is scrolled to the [direction](Rizzy.SwapStyleBuilder.Scroll(Rizzy.ScrollDirection,string).md#Rizzy.SwapStyleBuilder.Scroll(Rizzy.ScrollDirection,string).direction 'Rizzy.SwapStyleBuilder.Scroll(Rizzy.ScrollDirection, string).direction') of the content identified by the css cssSelector.