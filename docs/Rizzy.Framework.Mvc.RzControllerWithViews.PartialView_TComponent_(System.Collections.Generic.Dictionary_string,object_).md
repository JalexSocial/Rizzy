#### [Rizzy](index 'index')
### [Rizzy.Framework.Mvc](Rizzy.Framework.Mvc 'Rizzy.Framework.Mvc').[RzControllerWithViews](Rizzy.Framework.Mvc.RzControllerWithViews 'Rizzy.Framework.Mvc.RzControllerWithViews')

## RzControllerWithViews.PartialView<TComponent>(Dictionary<string,object>) Method

Renders a Razor component without a layout

```csharp
public Microsoft.AspNetCore.Http.IResult PartialView<TComponent>(System.Collections.Generic.Dictionary<string,object?> data)
    where TComponent : Microsoft.AspNetCore.Components.IComponent;
```
#### Type parameters

<a name='Rizzy.Framework.Mvc.RzControllerWithViews.PartialView_TComponent_(System.Collections.Generic.Dictionary_string,object_).TComponent'></a>

`TComponent`
#### Parameters

<a name='Rizzy.Framework.Mvc.RzControllerWithViews.PartialView_TComponent_(System.Collections.Generic.Dictionary_string,object_).data'></a>

`data` [System.Collections.Generic.Dictionary&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')[System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')

Implements [PartialView&lt;TComponent&gt;(Dictionary&lt;string,object&gt;)](Rizzy.IRizzyService.PartialView_TComponent_(System.Collections.Generic.Dictionary_string,object_) 'Rizzy.IRizzyService.PartialView<TComponent>(System.Collections.Generic.Dictionary<string,object>)')

#### Returns
[Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult')