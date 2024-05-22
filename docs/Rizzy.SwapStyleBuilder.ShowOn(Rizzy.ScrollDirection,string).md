#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder')

## SwapStyleBuilder.ShowOn(ScrollDirection, string) Method

Specifies a CSS cssSelector to target for the swap operation, smoothly animating the scrollbar position to either the  
top or the bottom of the target element after the swap.

```csharp
public Rizzy.SwapStyleBuilder ShowOn(Rizzy.ScrollDirection direction, string? cssSelector=null);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilder.ShowOn(Rizzy.ScrollDirection,string).direction'></a>

`direction` [ScrollDirection](Rizzy.ScrollDirection 'Rizzy.ScrollDirection')

The scroll direction after swap.

<a name='Rizzy.SwapStyleBuilder.ShowOn(Rizzy.ScrollDirection,string).cssSelector'></a>

`cssSelector` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

Optional CSS cssSelector of the target element.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder')  
This [SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
Adds a show modifier with the specified CSS cssSelector and scroll direction. For example, if [cssSelector](Rizzy.SwapStyleBuilder.ShowOn(Rizzy.ScrollDirection,string)#Rizzy.SwapStyleBuilder.ShowOn(Rizzy.ScrollDirection,string).cssSelector 'Rizzy.SwapStyleBuilder.ShowOn(Rizzy.ScrollDirection, string).cssSelector')  
is ".item" and [direction](Rizzy.SwapStyleBuilder.ShowOn(Rizzy.ScrollDirection,string)#Rizzy.SwapStyleBuilder.ShowOn(Rizzy.ScrollDirection,string).direction 'Rizzy.SwapStyleBuilder.ShowOn(Rizzy.ScrollDirection, string).direction') is [Rizzy.ScrollDirection.top](https://docs.microsoft.com/en-us/dotnet/api/Rizzy.ScrollDirection.top 'Rizzy.ScrollDirection.top'), the modifier `show:.item:top`  
is added.