#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[SwapStyleBuilderExtension](Rizzy.SwapStyleBuilderExtension 'Rizzy.SwapStyleBuilderExtension')

## SwapStyleBuilderExtension.IncludeTitle(this SwapStyle) Method

Includes the document title from the swap response in the current page.

```csharp
public static Rizzy.SwapStyleBuilder IncludeTitle(this Rizzy.SwapStyle style);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilderExtension.IncludeTitle(thisRizzy.SwapStyle).style'></a>

`style` [SwapStyle](Rizzy.SwapStyle 'Rizzy.SwapStyle')

The swap style.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder')  
A [SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
This method ensures the title of the document is updated according to the swap response by removing any  
ignoreTitle modifiers, effectively setting `ignoreTitle:false`.