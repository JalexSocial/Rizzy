#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')

## SwapStyleBuilder.ScrollBottom(string) Method

Sets the content scrollbar position to the bottom of the swapped content after a swap.

```csharp
public Rizzy.SwapStyleBuilder ScrollBottom(string? cssSelector=null);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilder.ScrollBottom(string).cssSelector'></a>

`cssSelector` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

Optional CSS cssSelector of the target element.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')  
This [SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
This method adds the modifier `scroll:bottom` to the swap commands, instructing the page to scroll to  
the bottom of the content after content is swapped immediately and without animation. If css [cssSelector](Rizzy.SwapStyleBuilder.ScrollBottom(string).md#Rizzy.SwapStyleBuilder.ScrollBottom(string).cssSelector 'Rizzy.SwapStyleBuilder.ScrollBottom(string).cssSelector')  
is present then the page is scrolled to the bottom of the content identified by the css cssSelector.