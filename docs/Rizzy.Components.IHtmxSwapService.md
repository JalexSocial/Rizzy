#### [Rizzy](index 'index')
### [Rizzy.Components](Rizzy.Components 'Rizzy.Components')

## IHtmxSwapService Interface

```csharp
public interface IHtmxSwapService
```

Derived  
&#8627; [HtmxSwapService](Rizzy.Components.HtmxSwapService 'Rizzy.Components.HtmxSwapService')

| Properties | |
| :--- | :--- |
| [ContentAvailable](Rizzy.Components.IHtmxSwapService.ContentAvailable 'Rizzy.Components.IHtmxSwapService.ContentAvailable') | Is there any content available to render? |

| Methods | |
| :--- | :--- |
| [AddRawContent(string)](Rizzy.Components.IHtmxSwapService.AddRawContent(string) 'Rizzy.Components.IHtmxSwapService.AddRawContent(string)') | Adds raw HTML content to the service for later rendering. |
| [AddSwappableComponent&lt;TComponent&gt;(string, object, SwapStyle, string)](Rizzy.Components.IHtmxSwapService.AddSwappableComponent_TComponent_(string,object,Rizzy.Htmx.SwapStyle,string) 'Rizzy.Components.IHtmxSwapService.AddSwappableComponent<TComponent>(string, object, Rizzy.Htmx.SwapStyle, string)') | Adds a swappable Razor component to the service for later rendering. |
| [AddSwappableComponent&lt;TComponent&gt;(string, Dictionary&lt;string,object&gt;, SwapStyle, string)](Rizzy.Components.IHtmxSwapService.AddSwappableComponent_TComponent_(string,System.Collections.Generic.Dictionary_string,object_,Rizzy.Htmx.SwapStyle,string) 'Rizzy.Components.IHtmxSwapService.AddSwappableComponent<TComponent>(string, System.Collections.Generic.Dictionary<string,object>, Rizzy.Htmx.SwapStyle, string)') | Adds a swappable Razor component to the service for later rendering. |
| [AddSwappableContent(string, string, SwapStyle, string)](Rizzy.Components.IHtmxSwapService.AddSwappableContent(string,string,Rizzy.Htmx.SwapStyle,string) 'Rizzy.Components.IHtmxSwapService.AddSwappableContent(string, string, Rizzy.Htmx.SwapStyle, string)') | Adds string content to the service for later rendering. |
| [AddSwappableFragment(string, RenderFragment, SwapStyle, string)](Rizzy.Components.IHtmxSwapService.AddSwappableFragment(string,Microsoft.AspNetCore.Components.RenderFragment,Rizzy.Htmx.SwapStyle,string) 'Rizzy.Components.IHtmxSwapService.AddSwappableFragment(string, Microsoft.AspNetCore.Components.RenderFragment, Rizzy.Htmx.SwapStyle, string)') | Adds a RenderFragment to the service for later rendering. |
| [Clear()](Rizzy.Components.IHtmxSwapService.Clear() 'Rizzy.Components.IHtmxSwapService.Clear()') | Remove all content items from Swap Service |
| [RenderToFragment()](Rizzy.Components.IHtmxSwapService.RenderToFragment() 'Rizzy.Components.IHtmxSwapService.RenderToFragment()') | Renders all added content (components, fragments, and raw HTML) to a single RenderFragment. |
| [RenderToString()](Rizzy.Components.IHtmxSwapService.RenderToString() 'Rizzy.Components.IHtmxSwapService.RenderToString()') | Renders all added content (components, fragments, and raw HTML) to a string. |
