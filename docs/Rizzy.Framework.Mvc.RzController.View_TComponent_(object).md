#### [Rizzy](index.md 'index')
### [Rizzy.Framework.Mvc](Rizzy.Framework.Mvc.md 'Rizzy.Framework.Mvc').[RzController](Rizzy.Framework.Mvc.RzController.md 'Rizzy.Framework.Mvc.RzController')

## RzController.View<TComponent>(object) Method

Renders a view using the specified Razor component, optionally accepting dynamic data to pass to the component.

```csharp
public Microsoft.AspNetCore.Http.IResult View<TComponent>(object? data=null)
    where TComponent : Microsoft.AspNetCore.Components.IComponent;
```
#### Type parameters

<a name='Rizzy.Framework.Mvc.RzController.View_TComponent_(object).TComponent'></a>

`TComponent`

The type of the Razor component to render.
#### Parameters

<a name='Rizzy.Framework.Mvc.RzController.View_TComponent_(object).data'></a>

`data` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

Optional dynamic data to pass to the component. Defaults to null if not provided.

Implements [View&lt;TComponent&gt;(object)](Rizzy.IRizzyService.View_TComponent_(object).md 'Rizzy.IRizzyService.View<TComponent>(object)')

#### Returns
[Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult')  
An [Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult') that can render the specified component as a view.