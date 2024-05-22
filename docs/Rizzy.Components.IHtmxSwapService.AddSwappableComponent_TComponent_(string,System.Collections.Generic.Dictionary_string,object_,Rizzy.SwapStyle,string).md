#### [Rizzy](index.md 'index')
### [Rizzy.Components](Rizzy.Components.md 'Rizzy.Components').[IHtmxSwapService](Rizzy.Components.IHtmxSwapService.md 'Rizzy.Components.IHtmxSwapService')

## IHtmxSwapService.AddSwappableComponent<TComponent>(string, Dictionary<string,object>, SwapStyle, string) Method

Adds a swappable Razor component to the service for later rendering.

```csharp
void AddSwappableComponent<TComponent>(string targetId, System.Collections.Generic.Dictionary<string,object>? parameters=null, Rizzy.SwapStyle swapStyle=Rizzy.SwapStyle.outerHTML, string? selector=null)
    where TComponent : Microsoft.AspNetCore.Components.IComponent;
```
#### Type parameters

<a name='Rizzy.Components.IHtmxSwapService.AddSwappableComponent_TComponent_(string,System.Collections.Generic.Dictionary_string,object_,Rizzy.SwapStyle,string).TComponent'></a>

`TComponent`

The type of the Razor component to add.
#### Parameters

<a name='Rizzy.Components.IHtmxSwapService.AddSwappableComponent_TComponent_(string,System.Collections.Generic.Dictionary_string,object_,Rizzy.SwapStyle,string).targetId'></a>

`targetId` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The target DOM element ID where the component should be rendered.

<a name='Rizzy.Components.IHtmxSwapService.AddSwappableComponent_TComponent_(string,System.Collections.Generic.Dictionary_string,object_,Rizzy.SwapStyle,string).parameters'></a>

`parameters` [System.Collections.Generic.Dictionary&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')[System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')

Optional parameters to pass to the component.

<a name='Rizzy.Components.IHtmxSwapService.AddSwappableComponent_TComponent_(string,System.Collections.Generic.Dictionary_string,object_,Rizzy.SwapStyle,string).swapStyle'></a>

`swapStyle` [SwapStyle](Rizzy.SwapStyle.md 'Rizzy.SwapStyle')

The style of content swap to apply.

<a name='Rizzy.Components.IHtmxSwapService.AddSwappableComponent_TComponent_(string,System.Collections.Generic.Dictionary_string,object_,Rizzy.SwapStyle,string).selector'></a>

`selector` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

A CSS selector to specify where to apply the swap.