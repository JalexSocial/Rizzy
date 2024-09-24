#### [Rizzy](index 'index')
### [Rizzy.Framework.Mvc](Rizzy.Framework.Mvc 'Rizzy.Framework.Mvc')

## RzController Class

Base controller for Rizzy that provides access to Razor Component views

```csharp
public class RzController : Microsoft.AspNetCore.Mvc.ControllerBase,
Rizzy.IRizzyService,
Microsoft.AspNetCore.Mvc.Filters.IActionFilter,
Microsoft.AspNetCore.Mvc.Filters.IFilterMetadata,
Microsoft.AspNetCore.Mvc.Filters.IAsyncActionFilter,
System.IDisposable
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [Microsoft.AspNetCore.Mvc.ControllerBase](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.ControllerBase 'Microsoft.AspNetCore.Mvc.ControllerBase') &#129106; RzController

Implements [IRizzyService](Rizzy.IRizzyService 'Rizzy.IRizzyService'), [Microsoft.AspNetCore.Mvc.Filters.IActionFilter](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.Filters.IActionFilter 'Microsoft.AspNetCore.Mvc.Filters.IActionFilter'), [Microsoft.AspNetCore.Mvc.Filters.IFilterMetadata](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.Filters.IFilterMetadata 'Microsoft.AspNetCore.Mvc.Filters.IFilterMetadata'), [Microsoft.AspNetCore.Mvc.Filters.IAsyncActionFilter](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.Filters.IAsyncActionFilter 'Microsoft.AspNetCore.Mvc.Filters.IAsyncActionFilter'), [System.IDisposable](https://docs.microsoft.com/en-us/dotnet/api/System.IDisposable 'System.IDisposable')

| Properties | |
| :--- | :--- |
| [CurrentActionUrl](Rizzy.Framework.Mvc.RzController.CurrentActionUrl 'Rizzy.Framework.Mvc.RzController.CurrentActionUrl') | Returns the current action method url as a possible Form callback url but may be overridden manually in any form handler method<br/>This value can be used inside of form Razor Component views |
| [Htmx](Rizzy.Framework.Mvc.RzController.Htmx 'Rizzy.Framework.Mvc.RzController.Htmx') | Gets the Htmx context for the current request. |
| [ViewContext](Rizzy.Framework.Mvc.RzController.ViewContext 'Rizzy.Framework.Mvc.RzController.ViewContext') | Gets the view context associated with the service. The view context contains information required for configuring and rendering views. |

| Methods | |
| :--- | :--- |
| [Dispose()](Rizzy.Framework.Mvc.RzController.Dispose() 'Rizzy.Framework.Mvc.RzController.Dispose()') | Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. |
| [Json(object)](Rizzy.Framework.Mvc.RzController.Json(object) 'Rizzy.Framework.Mvc.RzController.Json(object)') | Creates a [Microsoft.AspNetCore.Mvc.JsonResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.JsonResult 'Microsoft.AspNetCore.Mvc.JsonResult') object that serializes the specified [data](Rizzy.Framework.Mvc.RzController.Json(object)#Rizzy.Framework.Mvc.RzController.Json(object).data 'Rizzy.Framework.Mvc.RzController.Json(object).data') object<br/>to JSON. |
| [Json(object, object)](Rizzy.Framework.Mvc.RzController.Json(object,object) 'Rizzy.Framework.Mvc.RzController.Json(object, object)') | Creates a [Microsoft.AspNetCore.Mvc.JsonResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.JsonResult 'Microsoft.AspNetCore.Mvc.JsonResult') object that serializes the specified [data](Rizzy.Framework.Mvc.RzController.Json(object,object)#Rizzy.Framework.Mvc.RzController.Json(object,object).data 'Rizzy.Framework.Mvc.RzController.Json(object, object).data') object<br/>to JSON. |
| [OnActionExecuted(ActionExecutedContext)](Rizzy.Framework.Mvc.RzController.OnActionExecuted(Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext) 'Rizzy.Framework.Mvc.RzController.OnActionExecuted(Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext)') | Called after the action method is invoked. |
| [OnActionExecuting(ActionExecutingContext)](Rizzy.Framework.Mvc.RzController.OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext) 'Rizzy.Framework.Mvc.RzController.OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext)') | Called before the action method is invoked. |
| [OnActionExecutionAsync(ActionExecutingContext, ActionExecutionDelegate)](Rizzy.Framework.Mvc.RzController.OnActionExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext,Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate) 'Rizzy.Framework.Mvc.RzController.OnActionExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext, Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate)') | Called before the action method is invoked. |
| [PartialView(RenderFragment)](Rizzy.Framework.Mvc.RzController.PartialView(Microsoft.AspNetCore.Components.RenderFragment) 'Rizzy.Framework.Mvc.RzController.PartialView(Microsoft.AspNetCore.Components.RenderFragment)') | Renders a Razor component without a layout from a RenderFragment |
| [PartialView&lt;TComponent&gt;(object)](Rizzy.Framework.Mvc.RzController.PartialView_TComponent_(object) 'Rizzy.Framework.Mvc.RzController.PartialView<TComponent>(object)') | Renders a partial view using the specified Razor component, optionally accepting dynamic data to pass to the component.<br/>This method is intended for rendering components without a layout, suitable for inclusion in other views. |
| [PartialView&lt;TComponent&gt;(Dictionary&lt;string,object&gt;)](Rizzy.Framework.Mvc.RzController.PartialView_TComponent_(System.Collections.Generic.Dictionary_string,object_) 'Rizzy.Framework.Mvc.RzController.PartialView<TComponent>(System.Collections.Generic.Dictionary<string,object>)') | Renders a Razor component without a layout |
| [View&lt;TComponent&gt;(object)](Rizzy.Framework.Mvc.RzController.View_TComponent_(object) 'Rizzy.Framework.Mvc.RzController.View<TComponent>(object)') | Renders a view using the specified Razor component, optionally accepting dynamic data to pass to the component. |
| [View&lt;TComponent&gt;(Dictionary&lt;string,object&gt;)](Rizzy.Framework.Mvc.RzController.View_TComponent_(System.Collections.Generic.Dictionary_string,object_) 'Rizzy.Framework.Mvc.RzController.View<TComponent>(System.Collections.Generic.Dictionary<string,object>)') | Renders a view using the specified Razor component with explicitly provided data in the form of a dictionary. |
