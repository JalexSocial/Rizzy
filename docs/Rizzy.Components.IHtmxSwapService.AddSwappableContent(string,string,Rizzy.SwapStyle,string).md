#### [Rizzy](index.md 'index')
### [Rizzy.Components](Rizzy.Components.md 'Rizzy.Components').[IHtmxSwapService](Rizzy.Components.IHtmxSwapService.md 'Rizzy.Components.IHtmxSwapService')

## IHtmxSwapService.AddSwappableContent(string, string, SwapStyle, string) Method

Adds string content to the service for later rendering.

```csharp
void AddSwappableContent(string targetId, string content, Rizzy.SwapStyle swapStyle=Rizzy.SwapStyle.outerHTML, string? selector=null);
```
#### Parameters

<a name='Rizzy.Components.IHtmxSwapService.AddSwappableContent(string,string,Rizzy.SwapStyle,string).targetId'></a>

`targetId` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The target DOM element ID where the fragment should be rendered.

<a name='Rizzy.Components.IHtmxSwapService.AddSwappableContent(string,string,Rizzy.SwapStyle,string).content'></a>

`content` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The RenderFragment to add.

<a name='Rizzy.Components.IHtmxSwapService.AddSwappableContent(string,string,Rizzy.SwapStyle,string).swapStyle'></a>

`swapStyle` [SwapStyle](Rizzy.SwapStyle.md 'Rizzy.SwapStyle')

The style of content swap to apply.

<a name='Rizzy.Components.IHtmxSwapService.AddSwappableContent(string,string,Rizzy.SwapStyle,string).selector'></a>

`selector` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

A CSS selector to specify where to apply the swap.