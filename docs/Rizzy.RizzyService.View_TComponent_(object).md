#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[RizzyService](Rizzy.RizzyService 'Rizzy.RizzyService')

## RizzyService.View<TComponent>(object) Method

Renders a view using the specified Razor component, optionally accepting dynamic data to pass to the component.

```csharp
public Microsoft.AspNetCore.Http.IResult View<TComponent>(object? data=null)
    where TComponent : Microsoft.AspNetCore.Components.IComponent;
```
#### Type parameters

<a name='Rizzy.RizzyService.View_TComponent_(object).TComponent'></a>

`TComponent`

The type of the Razor component to render.
#### Parameters

<a name='Rizzy.RizzyService.View_TComponent_(object).data'></a>

`data` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

Optional dynamic data to pass to the component. Defaults to null if not provided.

Implements [View&lt;TComponent&gt;(object)](Rizzy.IRizzyService.View_TComponent_(object) 'Rizzy.IRizzyService.View<TComponent>(object)')

#### Returns
[Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult')  
An [Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult') that can render the specified component as a view.