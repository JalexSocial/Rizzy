#### [Rizzy](index.md 'index')
### [Rizzy.Components](Rizzy.Components.md 'Rizzy.Components')

## IHtmxSwapService Interface

```csharp
public interface IHtmxSwapService
```

Derived  
&#8627; [HtmxSwapService](Rizzy.Components.HtmxSwapService.md 'Rizzy.Components.HtmxSwapService')

| Properties | |
| :--- | :--- |
| [ContentAvailable](Rizzy.Components.IHtmxSwapService.ContentAvailable.md 'Rizzy.Components.IHtmxSwapService.ContentAvailable') | Is there any content available to render? |

| Methods | |
| :--- | :--- |
| [AddRawContent(string)](Rizzy.Components.IHtmxSwapService.AddRawContent(string).md 'Rizzy.Components.IHtmxSwapService.AddRawContent(string)') | Adds raw HTML content to the service for later rendering. |
| [AddSwappableComponent&lt;TComponent&gt;(string, Dictionary&lt;string,object&gt;, SwapStyle, string)](Rizzy.Components.IHtmxSwapService.AddSwappableComponent_TComponent_(string,System.Collections.Generic.Dictionary_string,object_,Rizzy.SwapStyle,string).md 'Rizzy.Components.IHtmxSwapService.AddSwappableComponent<TComponent>(string, System.Collections.Generic.Dictionary<string,object>, Rizzy.SwapStyle, string)') | Adds a swappable Razor component to the service for later rendering. |
| [AddSwappableContent(string, string, SwapStyle, string)](Rizzy.Components.IHtmxSwapService.AddSwappableContent(string,string,Rizzy.SwapStyle,string).md 'Rizzy.Components.IHtmxSwapService.AddSwappableContent(string, string, Rizzy.SwapStyle, string)') | Adds string content to the service for later rendering. |
| [AddSwappableFragment(string, RenderFragment, SwapStyle, string)](Rizzy.Components.IHtmxSwapService.AddSwappableFragment(string,Microsoft.AspNetCore.Components.RenderFragment,Rizzy.SwapStyle,string).md 'Rizzy.Components.IHtmxSwapService.AddSwappableFragment(string, Microsoft.AspNetCore.Components.RenderFragment, Rizzy.SwapStyle, string)') | Adds a RenderFragment to the service for later rendering. |
| [Clear()](Rizzy.Components.IHtmxSwapService.Clear().md 'Rizzy.Components.IHtmxSwapService.Clear()') | Remove all content items from Swap Service |
| [RenderToFragment()](Rizzy.Components.IHtmxSwapService.RenderToFragment().md 'Rizzy.Components.IHtmxSwapService.RenderToFragment()') | Renders all added content (components, fragments, and raw HTML) to a single RenderFragment. |
| [RenderToString()](Rizzy.Components.IHtmxSwapService.RenderToString().md 'Rizzy.Components.IHtmxSwapService.RenderToString()') | Renders all added content (components, fragments, and raw HTML) to a string. |
