#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')

## SwapStyleBuilder.IgnoreTitle(bool) Method

Determines whether to ignore the document title in the swap response by appending the modifier  
`ignoreTitle:`.

```csharp
public Rizzy.SwapStyleBuilder IgnoreTitle(bool ignore=true);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilder.IgnoreTitle(bool).ignore'></a>

`ignore` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

Whether to ignore the title.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')  
This [SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
When set to true, the document title in the swap response will be ignored by adding the modifier  
`ignoreTitle:true`.  
This keeps the current title unchanged regardless of the incoming swap content's title tag.