#### [Rizzy](index 'index')
### [Rizzy.Framework.Mvc](Rizzy.Framework.Mvc 'Rizzy.Framework.Mvc')

## RzControllerWithViews Class

Base controller for Rizzy that provides access to Razor Component views and still provides standard MVC views

```csharp
public class RzControllerWithViews : Microsoft.AspNetCore.Mvc.Controller,
Rizzy.IRizzyService
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [Microsoft.AspNetCore.Mvc.ControllerBase](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.ControllerBase 'Microsoft.AspNetCore.Mvc.ControllerBase') &#129106; [Microsoft.AspNetCore.Mvc.Controller](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.Controller 'Microsoft.AspNetCore.Mvc.Controller') &#129106; RzControllerWithViews

Implements [IRizzyService](Rizzy.IRizzyService 'Rizzy.IRizzyService')

| Properties | |
| :--- | :--- |
| [CurrentActionUrl](Rizzy.Framework.Mvc.RzControllerWithViews.CurrentActionUrl 'Rizzy.Framework.Mvc.RzControllerWithViews.CurrentActionUrl') | Returns the current action method url as a possible Form callback url but may be overridden manually in any form handler method<br/>This value can be used inside of form Razor Component views |
| [Htmx](Rizzy.Framework.Mvc.RzControllerWithViews.Htmx 'Rizzy.Framework.Mvc.RzControllerWithViews.Htmx') | Gets the Htmx context for the current request. |
| [ViewContext](Rizzy.Framework.Mvc.RzControllerWithViews.ViewContext 'Rizzy.Framework.Mvc.RzControllerWithViews.ViewContext') | Gets the view context associated with the service. The view context contains information required for configuring and rendering views. |

| Methods | |
| :--- | :--- |
| [OnActionExecuting(ActionExecutingContext)](Rizzy.Framework.Mvc.RzControllerWithViews.OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext) 'Rizzy.Framework.Mvc.RzControllerWithViews.OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext)') | Called before the action method is invoked. |
| [PartialView(RenderFragment)](Rizzy.Framework.Mvc.RzControllerWithViews.PartialView(Microsoft.AspNetCore.Components.RenderFragment) 'Rizzy.Framework.Mvc.RzControllerWithViews.PartialView(Microsoft.AspNetCore.Components.RenderFragment)') | Renders a Razor component without a layout from a RenderFragment |
| [PartialView&lt;TComponent&gt;(object)](Rizzy.Framework.Mvc.RzControllerWithViews.PartialView_TComponent_(object) 'Rizzy.Framework.Mvc.RzControllerWithViews.PartialView<TComponent>(object)') | Renders a partial view using the specified Razor component, optionally accepting dynamic data to pass to the component.<br/>This method is intended for rendering components without a layout, suitable for inclusion in other views. |
| [PartialView&lt;TComponent&gt;(Dictionary&lt;string,object&gt;)](Rizzy.Framework.Mvc.RzControllerWithViews.PartialView_TComponent_(System.Collections.Generic.Dictionary_string,object_) 'Rizzy.Framework.Mvc.RzControllerWithViews.PartialView<TComponent>(System.Collections.Generic.Dictionary<string,object>)') | Renders a Razor component without a layout |
| [View&lt;TComponent&gt;(object)](Rizzy.Framework.Mvc.RzControllerWithViews.View_TComponent_(object) 'Rizzy.Framework.Mvc.RzControllerWithViews.View<TComponent>(object)') | Renders a view using the specified Razor component, optionally accepting dynamic data to pass to the component. |
| [View&lt;TComponent&gt;(Dictionary&lt;string,object&gt;)](Rizzy.Framework.Mvc.RzControllerWithViews.View_TComponent_(System.Collections.Generic.Dictionary_string,object_) 'Rizzy.Framework.Mvc.RzControllerWithViews.View<TComponent>(System.Collections.Generic.Dictionary<string,object>)') | Renders a view using the specified Razor component with explicitly provided data in the form of a dictionary. |
