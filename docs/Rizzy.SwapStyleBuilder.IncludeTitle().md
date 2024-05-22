#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder')

## SwapStyleBuilder.IncludeTitle() Method

Includes the document title from the swap response in the current page.

```csharp
public Rizzy.SwapStyleBuilder IncludeTitle();
```

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder')  
This [SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
This method ensures the title of the document is updated according to the swap response by removing any  
ignoreTitle modifiers, effectively setting `ignoreTitle:false`.