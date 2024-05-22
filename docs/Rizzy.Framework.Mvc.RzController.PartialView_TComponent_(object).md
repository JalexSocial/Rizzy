#### [Rizzy](index.md 'index')
### [Rizzy.Framework.Mvc](Rizzy.Framework.Mvc.md 'Rizzy.Framework.Mvc').[RzController](Rizzy.Framework.Mvc.RzController.md 'Rizzy.Framework.Mvc.RzController')

## RzController.PartialView<TComponent>(object) Method

Renders a partial view using the specified Razor component, optionally accepting dynamic data to pass to the component.  
This method is intended for rendering components without a layout, suitable for inclusion in other views.

```csharp
public Microsoft.AspNetCore.Http.IResult PartialView<TComponent>(object? data=null)
    where TComponent : Microsoft.AspNetCore.Components.IComponent;
```
#### Type parameters

<a name='Rizzy.Framework.Mvc.RzController.PartialView_TComponent_(object).TComponent'></a>

`TComponent`

The type of the Razor component to render.
#### Parameters

<a name='Rizzy.Framework.Mvc.RzController.PartialView_TComponent_(object).data'></a>

`data` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

Optional dynamic data to pass to the component. Defaults to null if not provided.

Implements [PartialView&lt;TComponent&gt;(object)](Rizzy.IRizzyService.PartialView_TComponent_(object).md 'Rizzy.IRizzyService.PartialView<TComponent>(object)')

#### Returns
[Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult')  
An [Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult') that can render the specified component as a partial view.