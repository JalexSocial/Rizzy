#### [Rizzy](index 'index')
### [Rizzy.Components](Rizzy.Components 'Rizzy.Components').[HtmxSwapService](Rizzy.Components.HtmxSwapService 'Rizzy.Components.HtmxSwapService')

## HtmxSwapService.AddSwappableComponent<TComponent>(string, Dictionary<string,object>, SwapStyle, string) Method

Adds a swappable Razor component to the service for later rendering.

```csharp
public void AddSwappableComponent<TComponent>(string targetId, System.Collections.Generic.Dictionary<string,object?>? parameters=null, Rizzy.Htmx.SwapStyle swapStyle=Rizzy.Htmx.SwapStyle.outerHTML, string? selector=null)
    where TComponent : Microsoft.AspNetCore.Components.IComponent;
```
#### Type parameters

<a name='Rizzy.Components.HtmxSwapService.AddSwappableComponent_TComponent_(string,System.Collections.Generic.Dictionary_string,object_,Rizzy.Htmx.SwapStyle,string).TComponent'></a>

`TComponent`

The type of the Razor component to add.
#### Parameters

<a name='Rizzy.Components.HtmxSwapService.AddSwappableComponent_TComponent_(string,System.Collections.Generic.Dictionary_string,object_,Rizzy.Htmx.SwapStyle,string).targetId'></a>

`targetId` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The target DOM element ID where the component should be rendered.

<a name='Rizzy.Components.HtmxSwapService.AddSwappableComponent_TComponent_(string,System.Collections.Generic.Dictionary_string,object_,Rizzy.Htmx.SwapStyle,string).parameters'></a>

`parameters` [System.Collections.Generic.Dictionary&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')[System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')

Optional parameters to pass to the component.

<a name='Rizzy.Components.HtmxSwapService.AddSwappableComponent_TComponent_(string,System.Collections.Generic.Dictionary_string,object_,Rizzy.Htmx.SwapStyle,string).swapStyle'></a>

`swapStyle` [Rizzy.Htmx.SwapStyle](https://docs.microsoft.com/en-us/dotnet/api/Rizzy.Htmx.SwapStyle 'Rizzy.Htmx.SwapStyle')

The style of content swap to apply.

<a name='Rizzy.Components.HtmxSwapService.AddSwappableComponent_TComponent_(string,System.Collections.Generic.Dictionary_string,object_,Rizzy.Htmx.SwapStyle,string).selector'></a>

`selector` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

A CSS selector to specify where to apply the swap.

Implements [AddSwappableComponent&lt;TComponent&gt;(string, Dictionary&lt;string,object&gt;, SwapStyle, string)](Rizzy.Components.IHtmxSwapService.AddSwappableComponent_TComponent_(string,System.Collections.Generic.Dictionary_string,object_,Rizzy.Htmx.SwapStyle,string) 'Rizzy.Components.IHtmxSwapService.AddSwappableComponent<TComponent>(string, System.Collections.Generic.Dictionary<string,object>, Rizzy.Htmx.SwapStyle, string)')