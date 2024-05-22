#### [Rizzy](index 'index')
### [Rizzy.Framework.Mvc](Rizzy.Framework.Mvc 'Rizzy.Framework.Mvc').[RzControllerWithViews](Rizzy.Framework.Mvc.RzControllerWithViews 'Rizzy.Framework.Mvc.RzControllerWithViews')

## RzControllerWithViews.View<TComponent>(Dictionary<string,object>) Method

Renders a view using the specified Razor component with explicitly provided data in the form of a dictionary.

```csharp
public Microsoft.AspNetCore.Http.IResult View<TComponent>(System.Collections.Generic.Dictionary<string,object?> data)
    where TComponent : Microsoft.AspNetCore.Components.IComponent;
```
#### Type parameters

<a name='Rizzy.Framework.Mvc.RzControllerWithViews.View_TComponent_(System.Collections.Generic.Dictionary_string,object_).TComponent'></a>

`TComponent`

The type of the Razor component to render.
#### Parameters

<a name='Rizzy.Framework.Mvc.RzControllerWithViews.View_TComponent_(System.Collections.Generic.Dictionary_string,object_).data'></a>

`data` [System.Collections.Generic.Dictionary&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')[System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')

A dictionary containing the data to pass to the component.

Implements [View&lt;TComponent&gt;(Dictionary&lt;string,object&gt;)](Rizzy.IRizzyService.View_TComponent_(System.Collections.Generic.Dictionary_string,object_) 'Rizzy.IRizzyService.View<TComponent>(System.Collections.Generic.Dictionary<string,object>)')

#### Returns
[Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult')  
An [Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult') that can render the specified component as a view.