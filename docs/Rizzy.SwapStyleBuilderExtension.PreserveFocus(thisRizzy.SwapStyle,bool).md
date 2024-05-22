#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[SwapStyleBuilderExtension](Rizzy.SwapStyleBuilderExtension 'Rizzy.SwapStyleBuilderExtension')

## SwapStyleBuilderExtension.PreserveFocus(this SwapStyle, bool) Method

Explicitly preserves focus between requests for inputs that have a defined id attribute without  
scrolling.

```csharp
public static Rizzy.SwapStyleBuilder PreserveFocus(this Rizzy.SwapStyle style, bool scroll=true);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilderExtension.PreserveFocus(thisRizzy.SwapStyle,bool).style'></a>

`style` [SwapStyle](Rizzy.SwapStyle 'Rizzy.SwapStyle')

The swap style.

<a name='Rizzy.SwapStyleBuilderExtension.PreserveFocus(thisRizzy.SwapStyle,bool).scroll'></a>

`scroll` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

Whether to scroll to current focus or preserve focus

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder')  
A [SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
Adds a modifier of `focus-scroll:false`