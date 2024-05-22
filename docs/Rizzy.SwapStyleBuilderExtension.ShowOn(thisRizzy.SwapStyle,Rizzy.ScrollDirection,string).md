#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[SwapStyleBuilderExtension](Rizzy.SwapStyleBuilderExtension 'Rizzy.SwapStyleBuilderExtension')

## SwapStyleBuilderExtension.ShowOn(this SwapStyle, ScrollDirection, string) Method

Specifies a css selector to dynamically target for the swap operation, with a scroll direction after the swap.

```csharp
public static Rizzy.SwapStyleBuilder ShowOn(this Rizzy.SwapStyle style, Rizzy.ScrollDirection direction, string? cssSelector=null);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilderExtension.ShowOn(thisRizzy.SwapStyle,Rizzy.ScrollDirection,string).style'></a>

`style` [SwapStyle](Rizzy.SwapStyle 'Rizzy.SwapStyle')

The swap style.

<a name='Rizzy.SwapStyleBuilderExtension.ShowOn(thisRizzy.SwapStyle,Rizzy.ScrollDirection,string).direction'></a>

`direction` [ScrollDirection](Rizzy.ScrollDirection 'Rizzy.ScrollDirection')

The scroll direction after swap.

<a name='Rizzy.SwapStyleBuilderExtension.ShowOn(thisRizzy.SwapStyle,Rizzy.ScrollDirection,string).cssSelector'></a>

`cssSelector` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

Optional css selector of the target element.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder')  
A [SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
Adds a show modifier with the specified CSS selector and scroll direction. For example, if [cssSelector](Rizzy.SwapStyleBuilderExtension.ShowOn(thisRizzy.SwapStyle,Rizzy.ScrollDirection,string)#Rizzy.SwapStyleBuilderExtension.ShowOn(thisRizzy.SwapStyle,Rizzy.ScrollDirection,string).cssSelector 'Rizzy.SwapStyleBuilderExtension.ShowOn(this Rizzy.SwapStyle, Rizzy.ScrollDirection, string).cssSelector')  
is ".item" and [direction](Rizzy.SwapStyleBuilderExtension.ShowOn(thisRizzy.SwapStyle,Rizzy.ScrollDirection,string)#Rizzy.SwapStyleBuilderExtension.ShowOn(thisRizzy.SwapStyle,Rizzy.ScrollDirection,string).direction 'Rizzy.SwapStyleBuilderExtension.ShowOn(this Rizzy.SwapStyle, Rizzy.ScrollDirection, string).direction') is [Rizzy.ScrollDirection.top](https://docs.microsoft.com/en-us/dotnet/api/Rizzy.ScrollDirection.top 'Rizzy.ScrollDirection.top'), the modifier `show:.item:top`  
is added.