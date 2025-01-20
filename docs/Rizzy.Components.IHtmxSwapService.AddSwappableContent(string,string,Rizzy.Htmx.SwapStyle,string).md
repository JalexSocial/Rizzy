#### [Rizzy](index 'index')
### [Rizzy.Components](Rizzy.Components 'Rizzy.Components').[IHtmxSwapService](Rizzy.Components.IHtmxSwapService 'Rizzy.Components.IHtmxSwapService')

## IHtmxSwapService.AddSwappableContent(string, string, SwapStyle, string) Method

Adds string content to the service for later rendering.

```csharp
void AddSwappableContent(string targetId, string content, Rizzy.Htmx.SwapStyle swapStyle=Rizzy.Htmx.SwapStyle.outerHTML, string? selector=null);
```
#### Parameters

<a name='Rizzy.Components.IHtmxSwapService.AddSwappableContent(string,string,Rizzy.Htmx.SwapStyle,string).targetId'></a>

`targetId` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The target DOM element ID where the fragment should be rendered.

<a name='Rizzy.Components.IHtmxSwapService.AddSwappableContent(string,string,Rizzy.Htmx.SwapStyle,string).content'></a>

`content` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The RenderFragment to add.

<a name='Rizzy.Components.IHtmxSwapService.AddSwappableContent(string,string,Rizzy.Htmx.SwapStyle,string).swapStyle'></a>

`swapStyle` [Rizzy.Htmx.SwapStyle](https://docs.microsoft.com/en-us/dotnet/api/Rizzy.Htmx.SwapStyle 'Rizzy.Htmx.SwapStyle')

The style of content swap to apply.

<a name='Rizzy.Components.IHtmxSwapService.AddSwappableContent(string,string,Rizzy.Htmx.SwapStyle,string).selector'></a>

`selector` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

A CSS selector to specify where to apply the swap.