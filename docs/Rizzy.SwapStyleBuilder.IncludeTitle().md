#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')

## SwapStyleBuilder.IncludeTitle() Method

Includes the document title from the swap response in the current page.

```csharp
public Rizzy.SwapStyleBuilder IncludeTitle();
```

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')  
This [SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
This method ensures the title of the document is updated according to the swap response by removing any  
ignoreTitle modifiers, effectively setting `ignoreTitle:false`.