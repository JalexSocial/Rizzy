#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[IRizzyService](Rizzy.IRizzyService 'Rizzy.IRizzyService')

## IRizzyService.PartialView<TComponent>(object) Method

Renders a partial view using the specified Razor component, optionally accepting dynamic data to pass to the component.  
This method is intended for rendering components without a layout, suitable for inclusion in other views.

```csharp
Microsoft.AspNetCore.Http.IResult PartialView<TComponent>(object? data=null)
    where TComponent : Microsoft.AspNetCore.Components.IComponent;
```
#### Type parameters

<a name='Rizzy.IRizzyService.PartialView_TComponent_(object).TComponent'></a>

`TComponent`

The type of the Razor component to render.
#### Parameters

<a name='Rizzy.IRizzyService.PartialView_TComponent_(object).data'></a>

`data` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

Optional dynamic data to pass to the component. Defaults to null if not provided.

#### Returns
[Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult')  
An [Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult') that can render the specified component as a partial view.