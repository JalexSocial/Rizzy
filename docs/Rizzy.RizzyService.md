#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy')

## RizzyService Class

Represents a proxy base for Rizzy services that facilitate access to Razor Component views. This class provides  
mechanisms to render both full and partial Razor views dynamically based on specified component types and parameters.

```csharp
public sealed class RizzyService :
Rizzy.IRizzyService
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; RizzyService

Implements [IRizzyService](Rizzy.IRizzyService 'Rizzy.IRizzyService')

| Constructors | |
| :--- | :--- |
| [RizzyService(IHttpContextAccessor)](Rizzy.RizzyService.RizzyService(Microsoft.AspNetCore.Http.IHttpContextAccessor) 'Rizzy.RizzyService.RizzyService(Microsoft.AspNetCore.Http.IHttpContextAccessor)') | Initializes a new instance of the RizzyService |

| Properties | |
| :--- | :--- |
| [CurrentActionUrl](Rizzy.RizzyService.CurrentActionUrl 'Rizzy.RizzyService.CurrentActionUrl') | Gets the current action method URL, which can be used as a callback URL in forms. This URL is automatically<br/>derived from the current HTTP request but can be manually overridden in form handler methods. It is particularly<br/>useful for specifying form action targets within Razor Component views. |

| Methods | |
| :--- | :--- |
| [PartialView(RenderFragment)](Rizzy.RizzyService.PartialView(Microsoft.AspNetCore.Components.RenderFragment) 'Rizzy.RizzyService.PartialView(Microsoft.AspNetCore.Components.RenderFragment)') | Renders a Razor component without a layout from a RenderFragment |
| [PartialView(RenderFragment[])](Rizzy.RizzyService.PartialView(Microsoft.AspNetCore.Components.RenderFragment[]) 'Rizzy.RizzyService.PartialView(Microsoft.AspNetCore.Components.RenderFragment[])') | Renders a Razor component without a layout from a RenderFragment |
| [PartialView&lt;TComponent&gt;(object)](Rizzy.RizzyService.PartialView_TComponent_(object) 'Rizzy.RizzyService.PartialView<TComponent>(object)') | Renders a partial view using the specified Razor component, optionally accepting dynamic data to pass to the component.<br/>This method is intended for rendering components without a layout, suitable for inclusion in other views. |
| [PartialView&lt;TComponent&gt;(Dictionary&lt;string,object&gt;)](Rizzy.RizzyService.PartialView_TComponent_(System.Collections.Generic.Dictionary_string,object_) 'Rizzy.RizzyService.PartialView<TComponent>(System.Collections.Generic.Dictionary<string,object>)') | Renders a partial view using the specified Razor component with explicitly provided data in the form of a dictionary.<br/>This method is intended for rendering components without a layout, suitable for inclusion in other views. |
| [View&lt;TComponent&gt;(object)](Rizzy.RizzyService.View_TComponent_(object) 'Rizzy.RizzyService.View<TComponent>(object)') | Renders a view using the specified Razor component, optionally accepting dynamic data to pass to the component. |
| [View&lt;TComponent&gt;(Dictionary&lt;string,object&gt;)](Rizzy.RizzyService.View_TComponent_(System.Collections.Generic.Dictionary_string,object_) 'Rizzy.RizzyService.View<TComponent>(System.Collections.Generic.Dictionary<string,object>)') | Renders a view using the specified Razor component with explicitly provided data in the form of a dictionary. |
