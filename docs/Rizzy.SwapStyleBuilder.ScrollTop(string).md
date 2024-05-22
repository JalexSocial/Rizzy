#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')

## SwapStyleBuilder.ScrollTop(string) Method

Sets the content scrollbar position to the top of the swapped content after a swap.

```csharp
public Rizzy.SwapStyleBuilder ScrollTop(string? cssSelector=null);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilder.ScrollTop(string).cssSelector'></a>

`cssSelector` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

Optional CSS cssSelector of the target element.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')  
This [SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
This method adds the modifier `scroll:top` to the swap commands, instructing the page to scroll to  
the top of the content after content is swapped immediately and without animation. If css [cssSelector](Rizzy.SwapStyleBuilder.ScrollTop(string).md#Rizzy.SwapStyleBuilder.ScrollTop(string).cssSelector 'Rizzy.SwapStyleBuilder.ScrollTop(string).cssSelector')  
is present then the page is scrolled to the top of the content identified by the css cssSelector.