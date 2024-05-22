#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')

## SwapStyleBuilder.ScrollFocus(bool) Method

Allows you to specify that htmx should scroll to the focused element when a request completes.  
htmx preserves focus between requests for inputs that have a defined id attribute. By  
default htmx prevents auto-scrolling to focused inputs between requests which can be  
unwanted behavior on longer requests when the user has already scrolled away.

```csharp
public Rizzy.SwapStyleBuilder ScrollFocus(bool scroll=true);
```
#### Parameters

<a name='Rizzy.SwapStyleBuilder.ScrollFocus(bool).scroll'></a>

`scroll` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

Whether to scroll to the focus element.

#### Returns
[SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder')  
This [SwapStyleBuilder](Rizzy.SwapStyleBuilder.md 'Rizzy.SwapStyleBuilder') object instance.

### Remarks
[scroll](Rizzy.SwapStyleBuilder.ScrollFocus(bool).md#Rizzy.SwapStyleBuilder.ScrollFocus(bool).scroll 'Rizzy.SwapStyleBuilder.ScrollFocus(bool).scroll') when true will be `focus-scroll:true`, otherwise when false  
            will be `focus-scroll:false`