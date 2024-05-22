#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[SwapStyleBuilderExtension](Rizzy.SwapStyleBuilderExtension 'Rizzy.SwapStyleBuilderExtension')

## SwapStyleBuilderExtension.ScrollFocus(this SwapStyle, bool) Method

Allows you to specify that htmx should scroll to the focused element when a request completes.  
htmx preserves focus between requests for inputs that have a defined id attribute. By  
default htmx prevents auto-scrolling to focused inputs between requests which can be  
unwanted behavior on longer requests when the user has already scrolled away.

```csharp
public static Rizzy.SwapStyleBuilder ScrollFocus(this Rizzy.SwapStyle style, bool scroll=true);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilderExtension.ScrollFocus(thisRizzy.SwapStyle,bool).style'></a>

`style` [SwapStyle](Rizzy.SwapStyle 'Rizzy.SwapStyle')

The swap style.

<a name='Rizzy.SwapStyleBuilderExtension.ScrollFocus(thisRizzy.SwapStyle,bool).scroll'></a>

`scroll` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

Whether to scroll to the focus element.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder')  
A [SwapStyleBuilder](Rizzy.SwapStyleBuilder 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
[scroll](Rizzy.SwapStyleBuilderExtension.ScrollFocus(thisRizzy.SwapStyle,bool)#Rizzy.SwapStyleBuilderExtension.ScrollFocus(thisRizzy.SwapStyle,bool).scroll 'Rizzy.SwapStyleBuilderExtension.ScrollFocus(this Rizzy.SwapStyle, bool).scroll') when true will be `focus-scroll:true`, otherwise when false  
            will be `focus-scroll:false`