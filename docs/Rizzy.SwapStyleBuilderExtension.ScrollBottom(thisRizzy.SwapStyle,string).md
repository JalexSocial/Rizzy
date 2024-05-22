#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[SwapStyleBuilderExtension](Rizzy.SwapStyleBuilderExtension 'Rizzy.SwapStyleBuilderExtension')

## SwapStyleBuilderExtension.ScrollBottom(this SwapStyle, string) Method

Sets the content scrollbar position to the bottom of the swapped content after a swap.

```csharp
public static Rizzy.SwapStyleBuilder ScrollBottom(this Rizzy.SwapStyle style, string? cssSelector=null);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilderExtension.ScrollBottom(thisRizzy.SwapStyle,string).style'></a>

`style` [SwapStyle](Rizzy.SwapStyle 'Rizzy.SwapStyle')

The swap style.

<a name='Rizzy.SwapStyleBuilderExtension.ScrollBottom(thisRizzy.SwapStyle,string).cssSelector'></a>

`cssSelector` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

Optional css selector of the target element.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder')  
A [SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
This method adds the modifier `scroll:bottom` to the swap commands, instructing the page to scroll to  
the bottom of the content after content is swapped immediately and without animation. If css [cssSelector](Rizzy.SwapStyleBuilderExtension.ScrollBottom(thisRizzy.SwapStyle,string)#Rizzy.SwapStyleBuilderExtension.ScrollBottom(thisRizzy.SwapStyle,string).cssSelector 'Rizzy.SwapStyleBuilderExtension.ScrollBottom(this Rizzy.SwapStyle, string).cssSelector')  
is present then the page is scrolled to the bottom of the content identified by the css cssSelector.