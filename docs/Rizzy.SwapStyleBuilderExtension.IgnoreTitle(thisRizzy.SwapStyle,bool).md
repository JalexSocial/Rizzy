#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[SwapStyleBuilderExtension](Rizzy.SwapStyleBuilderExtension 'Rizzy.SwapStyleBuilderExtension')

## SwapStyleBuilderExtension.IgnoreTitle(this SwapStyle, bool) Method

Determines whether to ignore the document title in the swap response by appending the modifier  
`ignoreTitle:`.

```csharp
public static Rizzy.SwapStyleBuilder IgnoreTitle(this Rizzy.SwapStyle style, bool ignore=true);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilderExtension.IgnoreTitle(thisRizzy.SwapStyle,bool).style'></a>

`style` [SwapStyle](Rizzy.SwapStyle 'Rizzy.SwapStyle')

The swap style.

<a name='Rizzy.SwapStyleBuilderExtension.IgnoreTitle(thisRizzy.SwapStyle,bool).ignore'></a>

`ignore` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

Whether to ignore the title.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder')  
A [SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
When set to true, the document title in the swap response will be ignored by adding the modifier  
`ignoreTitle:true`.  
This keeps the current title unchanged regardless of the incoming swap content's title tag.