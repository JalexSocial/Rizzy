#### [Rizzy](index 'index')
### [Rizzy.Components](Rizzy.Components 'Rizzy.Components')

## HtmxSwapService Class

Service for managing dynamic content swaps in a Blazor application.  
Allows for adding Razor components, RenderFragments, and raw HTML content,  
and provides a mechanism to render them within a specified layout or context.

```csharp
public class HtmxSwapService :
Rizzy.Components.IHtmxSwapService
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; HtmxSwapService

Implements [IHtmxSwapService](Rizzy.Components.IHtmxSwapService 'Rizzy.Components.IHtmxSwapService')

| Properties | |
| :--- | :--- |
| [ContentAvailable](Rizzy.Components.HtmxSwapService.ContentAvailable 'Rizzy.Components.HtmxSwapService.ContentAvailable') | Is there any content available to render? |

| Methods | |
| :--- | :--- |
| [AddRawContent(string)](Rizzy.Components.HtmxSwapService.AddRawContent(string) 'Rizzy.Components.HtmxSwapService.AddRawContent(string)') | Adds raw HTML content to the service for later rendering. |
| [AddSwappableComponent&lt;TComponent&gt;(string, object, SwapStyle, string)](Rizzy.Components.HtmxSwapService.AddSwappableComponent_TComponent_(string,object,Rizzy.Htmx.SwapStyle,string) 'Rizzy.Components.HtmxSwapService.AddSwappableComponent<TComponent>(string, object, Rizzy.Htmx.SwapStyle, string)') | Adds a swappable Razor component to the service for later rendering. |
| [AddSwappableComponent&lt;TComponent&gt;(string, Dictionary&lt;string,object&gt;, SwapStyle, string)](Rizzy.Components.HtmxSwapService.AddSwappableComponent_TComponent_(string,System.Collections.Generic.Dictionary_string,object_,Rizzy.Htmx.SwapStyle,string) 'Rizzy.Components.HtmxSwapService.AddSwappableComponent<TComponent>(string, System.Collections.Generic.Dictionary<string,object>, Rizzy.Htmx.SwapStyle, string)') | Adds a swappable Razor component to the service for later rendering. |
| [AddSwappableContent(string, string, SwapStyle, string)](Rizzy.Components.HtmxSwapService.AddSwappableContent(string,string,Rizzy.Htmx.SwapStyle,string) 'Rizzy.Components.HtmxSwapService.AddSwappableContent(string, string, Rizzy.Htmx.SwapStyle, string)') | Adds string content to the service for later rendering. |
| [AddSwappableFragment(string, RenderFragment, SwapStyle, string)](Rizzy.Components.HtmxSwapService.AddSwappableFragment(string,Microsoft.AspNetCore.Components.RenderFragment,Rizzy.Htmx.SwapStyle,string) 'Rizzy.Components.HtmxSwapService.AddSwappableFragment(string, Microsoft.AspNetCore.Components.RenderFragment, Rizzy.Htmx.SwapStyle, string)') | Adds a RenderFragment to the service for later rendering. |
| [Clear()](Rizzy.Components.HtmxSwapService.Clear() 'Rizzy.Components.HtmxSwapService.Clear()') | Remove all content items from Swap Service |
| [RenderToFragment()](Rizzy.Components.HtmxSwapService.RenderToFragment() 'Rizzy.Components.HtmxSwapService.RenderToFragment()') | Renders all added content (components, fragments, and raw HTML) to a single RenderFragment. |
| [RenderToString()](Rizzy.Components.HtmxSwapService.RenderToString() 'Rizzy.Components.HtmxSwapService.RenderToString()') | Renders all added content (components, fragments, and raw HTML) to a string. |

| Events | |
| :--- | :--- |
| [ContentItemsUpdated](Rizzy.Components.HtmxSwapService.ContentItemsUpdated 'Rizzy.Components.HtmxSwapService.ContentItemsUpdated') | Triggered whenever content items are modified |
