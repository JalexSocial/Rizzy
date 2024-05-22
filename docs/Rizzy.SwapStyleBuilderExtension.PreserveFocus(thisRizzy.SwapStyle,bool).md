#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[SwapStyleBuilderExtension](Rizzy.SwapStyleBuilderExtension.md 'Rizzy.SwapStyleBuilderExtension')

## SwapStyleBuilderExtension.PreserveFocus(this SwapStyle, bool) Method

Explicitly preserves focus between requests for inputs that have a defined id attribute without  
scrolling.

```csharp
public static Rizzy.SwapStyleBuilder PreserveFocus(this Rizzy.SwapStyle style, bool scroll=true);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilderExtension.PreserveFocus(thisRizzy.SwapStyle,bool).style'></a>

`style` [SwapStyle](Rizzy.SwapStyle.md 'Rizzy.SwapStyle')

The swap style.

<a name='Rizzy.SwapStyleBuilderExtension.PreserveFocus(thisRizzy.SwapStyle,bool).scroll'></a>

`scroll` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

Whether to scroll to current focus or preserve focus

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')  
A [SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
Adds a modifier of `focus-scroll:false`