#### [Rizzy](index 'index')
### [Rizzy.Components](Rizzy.Components 'Rizzy.Components').[HtmxSwapService](Rizzy.Components.HtmxSwapService 'Rizzy.Components.HtmxSwapService')

## HtmxSwapService.AddSwappableFragment(string, RenderFragment, SwapStyle, string) Method

Adds a RenderFragment to the service for later rendering.

```csharp
public void AddSwappableFragment(string targetId, Microsoft.AspNetCore.Components.RenderFragment renderFragment, Rizzy.Htmx.SwapStyle swapStyle=Rizzy.Htmx.SwapStyle.outerHTML, string? selector=null);
```
#### Parameters

<a name='Rizzy.Components.HtmxSwapService.AddSwappableFragment(string,Microsoft.AspNetCore.Components.RenderFragment,Rizzy.Htmx.SwapStyle,string).targetId'></a>

`targetId` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The target DOM element ID where the fragment should be rendered.

<a name='Rizzy.Components.HtmxSwapService.AddSwappableFragment(string,Microsoft.AspNetCore.Components.RenderFragment,Rizzy.Htmx.SwapStyle,string).renderFragment'></a>

`renderFragment` [Microsoft.AspNetCore.Components.RenderFragment](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Components.RenderFragment 'Microsoft.AspNetCore.Components.RenderFragment')

The RenderFragment to add.

<a name='Rizzy.Components.HtmxSwapService.AddSwappableFragment(string,Microsoft.AspNetCore.Components.RenderFragment,Rizzy.Htmx.SwapStyle,string).swapStyle'></a>

`swapStyle` [Rizzy.Htmx.SwapStyle](https://docs.microsoft.com/en-us/dotnet/api/Rizzy.Htmx.SwapStyle 'Rizzy.Htmx.SwapStyle')

The style of content swap to apply.

<a name='Rizzy.Components.HtmxSwapService.AddSwappableFragment(string,Microsoft.AspNetCore.Components.RenderFragment,Rizzy.Htmx.SwapStyle,string).selector'></a>

`selector` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

A CSS selector to specify where to apply the swap.

Implements [AddSwappableFragment(string, RenderFragment, SwapStyle, string)](Rizzy.Components.IHtmxSwapService.AddSwappableFragment(string,Microsoft.AspNetCore.Components.RenderFragment,Rizzy.Htmx.SwapStyle,string) 'Rizzy.Components.IHtmxSwapService.AddSwappableFragment(string, Microsoft.AspNetCore.Components.RenderFragment, Rizzy.Htmx.SwapStyle, string)')