#### [Rizzy](index 'index')
### [Rizzy.Components](Rizzy.Components 'Rizzy.Components').[HtmxSwapService](Rizzy.Components.HtmxSwapService 'Rizzy.Components.HtmxSwapService')

## HtmxSwapService.AddSwappableComponent<TComponent>(string, object, SwapStyle, string) Method

Adds a swappable Razor component to the service for later rendering.

```csharp
public void AddSwappableComponent<TComponent>(string targetId, object? parameters=null, Rizzy.SwapStyle swapStyle=Rizzy.SwapStyle.outerHTML, string? selector=null)
    where TComponent : Microsoft.AspNetCore.Components.IComponent;
```
#### Type parameters

<a name='Rizzy.Components.HtmxSwapService.AddSwappableComponent_TComponent_(string,object,Rizzy.SwapStyle,string).TComponent'></a>

`TComponent`

The type of the Razor component to add.
#### Parameters

<a name='Rizzy.Components.HtmxSwapService.AddSwappableComponent_TComponent_(string,object,Rizzy.SwapStyle,string).targetId'></a>

`targetId` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The target DOM element ID where the component should be rendered.

<a name='Rizzy.Components.HtmxSwapService.AddSwappableComponent_TComponent_(string,object,Rizzy.SwapStyle,string).parameters'></a>

`parameters` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

Optional parameters to pass to the component.

<a name='Rizzy.Components.HtmxSwapService.AddSwappableComponent_TComponent_(string,object,Rizzy.SwapStyle,string).swapStyle'></a>

`swapStyle` [SwapStyle](Rizzy.SwapStyle 'Rizzy.SwapStyle')

The style of content swap to apply.

<a name='Rizzy.Components.HtmxSwapService.AddSwappableComponent_TComponent_(string,object,Rizzy.SwapStyle,string).selector'></a>

`selector` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

A CSS selector to specify where to apply the swap.

Implements [AddSwappableComponent&lt;TComponent&gt;(string, object, SwapStyle, string)](Rizzy.Components.IHtmxSwapService.AddSwappableComponent_TComponent_(string,object,Rizzy.SwapStyle,string) 'Rizzy.Components.IHtmxSwapService.AddSwappableComponent<TComponent>(string, object, Rizzy.SwapStyle, string)')