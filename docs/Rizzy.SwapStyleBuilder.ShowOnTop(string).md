#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder')

## SwapStyleBuilder.ShowOnTop(string) Method

Specifies that the swap should show the top of the element matching the CSS cssSelector.

```csharp
public Rizzy.SwapStyleBuilder ShowOnTop(string? cssSelector=null);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilder.ShowOnTop(string).cssSelector'></a>

`cssSelector` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

Optional CSS cssSelector of the target element.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder')  
This [SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
This method adds the modifier `show::top`, smoothly scrolling to the top of the element identified by  
[cssSelector](Rizzy.SwapStyleBuilder.ShowOnTop(string)#Rizzy.SwapStyleBuilder.ShowOnTop(string).cssSelector 'Rizzy.SwapStyleBuilder.ShowOnTop(string).cssSelector').