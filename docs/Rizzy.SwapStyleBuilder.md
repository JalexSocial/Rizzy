#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy')

## SwapStyleBuilder Class

A builder class for constructing a swap style command string for HTMX responses.

```csharp
public sealed class SwapStyleBuilder
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; SwapStyleBuilder

| Constructors | |
| :--- | :--- |
| [SwapStyleBuilder(SwapStyle)](Rizzy.SwapStyleBuilder.SwapStyleBuilder(Rizzy.SwapStyle).md 'Rizzy.SwapStyleBuilder.SwapStyleBuilder(Rizzy.SwapStyle)') | Initializes a new instance of the SwapStyleBuilder with a specified swap style. |

| Methods | |
| :--- | :--- |
| [AfterSettleDelay(TimeSpan)](Rizzy.SwapStyleBuilder.AfterSettleDelay(System.TimeSpan).md 'Rizzy.SwapStyleBuilder.AfterSettleDelay(System.TimeSpan)') | Modifies the amount of time that htmx will wait between the swap <br/>and the settle logic by including the modifier `settle:`. |
| [AfterSwapDelay(TimeSpan)](Rizzy.SwapStyleBuilder.AfterSwapDelay(System.TimeSpan).md 'Rizzy.SwapStyleBuilder.AfterSwapDelay(System.TimeSpan)') | Modifies the amount of time that htmx will wait after receiving a <br/>response to swap the content by including the modifier `swap:`. |
| [IgnoreTitle(bool)](Rizzy.SwapStyleBuilder.IgnoreTitle(bool).md 'Rizzy.SwapStyleBuilder.IgnoreTitle(bool)') | Determines whether to ignore the document title in the swap response by appending the modifier<br/>`ignoreTitle:`. |
| [IgnoreTransition()](Rizzy.SwapStyleBuilder.IgnoreTransition().md 'Rizzy.SwapStyleBuilder.IgnoreTransition()') | Explicitly ignores transition effects for the swap. |
| [IncludeTitle()](Rizzy.SwapStyleBuilder.IncludeTitle().md 'Rizzy.SwapStyleBuilder.IncludeTitle()') | Includes the document title from the swap response in the current page. |
| [IncludeTransition()](Rizzy.SwapStyleBuilder.IncludeTransition().md 'Rizzy.SwapStyleBuilder.IncludeTransition()') | Explicitly includes transition effects for the swap. |
| [PreserveFocus()](Rizzy.SwapStyleBuilder.PreserveFocus().md 'Rizzy.SwapStyleBuilder.PreserveFocus()') | Explicitly preserves focus between requests for inputs that have a defined id attribute without<br/>scrolling. |
| [Scroll(ScrollDirection, string)](Rizzy.SwapStyleBuilder.Scroll(Rizzy.ScrollDirection,string).md 'Rizzy.SwapStyleBuilder.Scroll(Rizzy.ScrollDirection, string)') | Specifies how to set the content scrollbar position after the swap and appends the modifier `scroll:`. |
| [ScrollBottom(string)](Rizzy.SwapStyleBuilder.ScrollBottom(string).md 'Rizzy.SwapStyleBuilder.ScrollBottom(string)') | Sets the content scrollbar position to the bottom of the swapped content after a swap. |
| [ScrollFocus(bool)](Rizzy.SwapStyleBuilder.ScrollFocus(bool).md 'Rizzy.SwapStyleBuilder.ScrollFocus(bool)') | Allows you to specify that htmx should scroll to the focused element when a request completes.<br/>htmx preserves focus between requests for inputs that have a defined id attribute. By<br/>default htmx prevents auto-scrolling to focused inputs between requests which can be<br/>unwanted behavior on longer requests when the user has already scrolled away. |
| [ScrollTop(string)](Rizzy.SwapStyleBuilder.ScrollTop(string).md 'Rizzy.SwapStyleBuilder.ScrollTop(string)') | Sets the content scrollbar position to the top of the swapped content after a swap. |
| [ShowNone()](Rizzy.SwapStyleBuilder.ShowNone().md 'Rizzy.SwapStyleBuilder.ShowNone()') | Turns off scrolling after swap. |
| [ShowOn(ScrollDirection, string)](Rizzy.SwapStyleBuilder.ShowOn(Rizzy.ScrollDirection,string).md 'Rizzy.SwapStyleBuilder.ShowOn(Rizzy.ScrollDirection, string)') | Specifies a CSS cssSelector to target for the swap operation, smoothly animating the scrollbar position to either the<br/>top or the bottom of the target element after the swap. |
| [ShowOnBottom(string)](Rizzy.SwapStyleBuilder.ShowOnBottom(string).md 'Rizzy.SwapStyleBuilder.ShowOnBottom(string)') | Specifies that the swap should show the bottom of the element matching the CSS cssSelector. |
| [ShowOnTop(string)](Rizzy.SwapStyleBuilder.ShowOnTop(string).md 'Rizzy.SwapStyleBuilder.ShowOnTop(string)') | Specifies that the swap should show the top of the element matching the CSS cssSelector. |
| [ShowWindow(ScrollDirection)](Rizzy.SwapStyleBuilder.ShowWindow(Rizzy.ScrollDirection).md 'Rizzy.SwapStyleBuilder.ShowWindow(Rizzy.ScrollDirection)') | Specifies that the swap should show in the window by smoothly scrolling to either the top or bottom of the window. |
| [ShowWindowBottom()](Rizzy.SwapStyleBuilder.ShowWindowBottom().md 'Rizzy.SwapStyleBuilder.ShowWindowBottom()') | Specifies that the swap should smoothly scroll to the bottom of the window. |
| [ShowWindowTop()](Rizzy.SwapStyleBuilder.ShowWindowTop().md 'Rizzy.SwapStyleBuilder.ShowWindowTop()') | Specifies that the swap should smoothly scroll to the top of the window. |
| [ToString()](Rizzy.SwapStyleBuilder.ToString().md 'Rizzy.SwapStyleBuilder.ToString()') | Returns a string that represents the current object. |
| [Transition(bool)](Rizzy.SwapStyleBuilder.Transition(bool).md 'Rizzy.SwapStyleBuilder.Transition(bool)') | Enables or disables transition effects for the swap by appending the modifier `transition:`. |
