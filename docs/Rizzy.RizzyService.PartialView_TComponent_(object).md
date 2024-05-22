#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[RizzyService](Rizzy.RizzyService 'Rizzy.RizzyService')

## RizzyService.PartialView<TComponent>(object) Method

Renders a partial view using the specified Razor component, optionally accepting dynamic data to pass to the component.  
This method is intended for rendering components without a layout, suitable for inclusion in other views.

```csharp
public Microsoft.AspNetCore.Http.IResult PartialView<TComponent>(object? data=null)
    where TComponent : Microsoft.AspNetCore.Components.IComponent;
```
#### Type parameters

<a name='Rizzy.RizzyService.PartialView_TComponent_(object).TComponent'></a>

`TComponent`

The type of the Razor component to render.
#### Parameters

<a name='Rizzy.RizzyService.PartialView_TComponent_(object).data'></a>

`data` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

Optional dynamic data to pass to the component. Defaults to null if not provided.

Implements [PartialView&lt;TComponent&gt;(object)](Rizzy.IRizzyService.PartialView_TComponent_(object) 'Rizzy.IRizzyService.PartialView<TComponent>(object)')

#### Returns
[Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult')  
An [Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult') that can render the specified component as a partial view.