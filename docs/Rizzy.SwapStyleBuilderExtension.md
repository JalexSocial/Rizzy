#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy')

## SwapStyleBuilderExtension Class

Extension methods for the SwapStyle enum to facilitate building swap style commands.

```csharp
public static class SwapStyleBuilderExtension
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; SwapStyleBuilderExtension

| Methods | |
| :--- | :--- |
| [AfterSettleDelay(this SwapStyle, TimeSpan)](Rizzy.SwapStyleBuilderExtension.AfterSettleDelay(thisRizzy.SwapStyle,System.TimeSpan) 'Rizzy.SwapStyleBuilderExtension.AfterSettleDelay(this Rizzy.SwapStyle, System.TimeSpan)') | Modifies the amount of time that htmx will wait between the swap <br/>and the settle logic by including the modifier `settle:`. |
| [AfterSwapDelay(this SwapStyle, TimeSpan)](Rizzy.SwapStyleBuilderExtension.AfterSwapDelay(thisRizzy.SwapStyle,System.TimeSpan) 'Rizzy.SwapStyleBuilderExtension.AfterSwapDelay(this Rizzy.SwapStyle, System.TimeSpan)') | Modifies the amount of time that htmx will wait after receiving a <br/>response to swap the content by including the modifier `swap:`. |
| [IgnoreTitle(this SwapStyle, bool)](Rizzy.SwapStyleBuilderExtension.IgnoreTitle(thisRizzy.SwapStyle,bool) 'Rizzy.SwapStyleBuilderExtension.IgnoreTitle(this Rizzy.SwapStyle, bool)') | Determines whether to ignore the document title in the swap response by appending the modifier<br/>`ignoreTitle:`. |
| [IgnoreTransition(this SwapStyle)](Rizzy.SwapStyleBuilderExtension.IgnoreTransition(thisRizzy.SwapStyle) 'Rizzy.SwapStyleBuilderExtension.IgnoreTransition(this Rizzy.SwapStyle)') | Explicitly ignores transition effects for the swap. |
| [IncludeTitle(this SwapStyle)](Rizzy.SwapStyleBuilderExtension.IncludeTitle(thisRizzy.SwapStyle) 'Rizzy.SwapStyleBuilderExtension.IncludeTitle(this Rizzy.SwapStyle)') | Includes the document title from the swap response in the current page. |
| [IncludeTransition(this SwapStyle)](Rizzy.SwapStyleBuilderExtension.IncludeTransition(thisRizzy.SwapStyle) 'Rizzy.SwapStyleBuilderExtension.IncludeTransition(this Rizzy.SwapStyle)') | Explicitly includes transition effects for the swap. |
| [PreserveFocus(this SwapStyle, bool)](Rizzy.SwapStyleBuilderExtension.PreserveFocus(thisRizzy.SwapStyle,bool) 'Rizzy.SwapStyleBuilderExtension.PreserveFocus(this Rizzy.SwapStyle, bool)') | Explicitly preserves focus between requests for inputs that have a defined id attribute without<br/>scrolling. |
| [Scroll(this SwapStyle, ScrollDirection, string)](Rizzy.SwapStyleBuilderExtension.Scroll(thisRizzy.SwapStyle,Rizzy.ScrollDirection,string) 'Rizzy.SwapStyleBuilderExtension.Scroll(this Rizzy.SwapStyle, Rizzy.ScrollDirection, string)') | Specifies how to set the content scrollbar position after the swap and appends the modifier `scroll:`. |
| [ScrollBottom(this SwapStyle, string)](Rizzy.SwapStyleBuilderExtension.ScrollBottom(thisRizzy.SwapStyle,string) 'Rizzy.SwapStyleBuilderExtension.ScrollBottom(this Rizzy.SwapStyle, string)') | Sets the content scrollbar position to the bottom of the swapped content after a swap. |
| [ScrollFocus(this SwapStyle, bool)](Rizzy.SwapStyleBuilderExtension.ScrollFocus(thisRizzy.SwapStyle,bool) 'Rizzy.SwapStyleBuilderExtension.ScrollFocus(this Rizzy.SwapStyle, bool)') | Allows you to specify that htmx should scroll to the focused element when a request completes.<br/>htmx preserves focus between requests for inputs that have a defined id attribute. By<br/>default htmx prevents auto-scrolling to focused inputs between requests which can be<br/>unwanted behavior on longer requests when the user has already scrolled away. |
| [ScrollTop(this SwapStyle, string)](Rizzy.SwapStyleBuilderExtension.ScrollTop(thisRizzy.SwapStyle,string) 'Rizzy.SwapStyleBuilderExtension.ScrollTop(this Rizzy.SwapStyle, string)') | Sets the content scrollbar position to the top of the swapped content after a swap. |
| [ShowNone(this SwapStyle)](Rizzy.SwapStyleBuilderExtension.ShowNone(thisRizzy.SwapStyle) 'Rizzy.SwapStyleBuilderExtension.ShowNone(this Rizzy.SwapStyle)') | Turns off scrolling after swap. |
| [ShowOn(this SwapStyle, ScrollDirection, string)](Rizzy.SwapStyleBuilderExtension.ShowOn(thisRizzy.SwapStyle,Rizzy.ScrollDirection,string) 'Rizzy.SwapStyleBuilderExtension.ShowOn(this Rizzy.SwapStyle, Rizzy.ScrollDirection, string)') | Specifies a css selector to dynamically target for the swap operation, with a scroll direction after the swap. |
| [ShowOnBottom(this SwapStyle, string)](Rizzy.SwapStyleBuilderExtension.ShowOnBottom(thisRizzy.SwapStyle,string) 'Rizzy.SwapStyleBuilderExtension.ShowOnBottom(this Rizzy.SwapStyle, string)') | Specifies that the swap should show the element matching the css selector at the bottom of the window. |
| [ShowOnTop(this SwapStyle, string)](Rizzy.SwapStyleBuilderExtension.ShowOnTop(thisRizzy.SwapStyle,string) 'Rizzy.SwapStyleBuilderExtension.ShowOnTop(this Rizzy.SwapStyle, string)') | Specifies that the swap should show the element matching the css selector at the top of the window. |
| [ShowWindow(this SwapStyle, ScrollDirection)](Rizzy.SwapStyleBuilderExtension.ShowWindow(thisRizzy.SwapStyle,Rizzy.ScrollDirection) 'Rizzy.SwapStyleBuilderExtension.ShowWindow(this Rizzy.SwapStyle, Rizzy.ScrollDirection)') | Specifies that the swap should show in the window by smoothly scrolling to either the top or bottom of the window. |
| [ShowWindowBottom(this SwapStyle)](Rizzy.SwapStyleBuilderExtension.ShowWindowBottom(thisRizzy.SwapStyle) 'Rizzy.SwapStyleBuilderExtension.ShowWindowBottom(this Rizzy.SwapStyle)') | Specifies that the swap should smoothly scroll to the bottom of the window. |
| [ShowWindowTop(this SwapStyle)](Rizzy.SwapStyleBuilderExtension.ShowWindowTop(thisRizzy.SwapStyle) 'Rizzy.SwapStyleBuilderExtension.ShowWindowTop(this Rizzy.SwapStyle)') | Specifies that the swap should smoothly scroll to the top of the window. |
| [Transition(this SwapStyle, bool)](Rizzy.SwapStyleBuilderExtension.Transition(thisRizzy.SwapStyle,bool) 'Rizzy.SwapStyleBuilderExtension.Transition(this Rizzy.SwapStyle, bool)') | Enables or disables transition effects for the swap by appending the modifier `transition:{show}`. |
