#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy')

## RizzyService Class

Represents a proxy base for Rizzy services that facilitate access to Razor Component views. This class provides  
mechanisms to render both full and partial Razor views dynamically based on specified component types and parameters.

```csharp
public sealed class RizzyService :
Rizzy.IRizzyService
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; RizzyService

Implements [IRizzyService](Rizzy.IRizzyService.md 'Rizzy.IRizzyService')

| Constructors | |
| :--- | :--- |
| [RizzyService(RzViewContext)](Rizzy.RizzyService.RizzyService(Rizzy.RzViewContext).md 'Rizzy.RizzyService.RizzyService(Rizzy.RzViewContext)') | Initializes a new instance of the RizzyService with a specified view context. |

| Properties | |
| :--- | :--- |
| [CurrentActionUrl](Rizzy.RizzyService.CurrentActionUrl.md 'Rizzy.RizzyService.CurrentActionUrl') | Gets the current action method URL, which can be used as a callback URL in forms. This URL is automatically<br/>derived from the current HTTP request but can be manually overridden in form handler methods. It is particularly<br/>useful for specifying form action targets within Razor Component views. |
| [ViewContext](Rizzy.RizzyService.ViewContext.md 'Rizzy.RizzyService.ViewContext') | Gets the view context associated with the service. The view context contains information required for configuring and rendering views. |

| Methods | |
| :--- | :--- |
| [PartialView&lt;TComponent&gt;(object)](Rizzy.RizzyService.PartialView_TComponent_(object).md 'Rizzy.RizzyService.PartialView<TComponent>(object)') | Renders a partial view using the specified Razor component, optionally accepting dynamic data to pass to the component.<br/>This method is intended for rendering components without a layout, suitable for inclusion in other views. |
| [PartialView&lt;TComponent&gt;(Dictionary&lt;string,object&gt;)](Rizzy.RizzyService.PartialView_TComponent_(System.Collections.Generic.Dictionary_string,object_).md 'Rizzy.RizzyService.PartialView<TComponent>(System.Collections.Generic.Dictionary<string,object>)') | Renders a partial view using the specified Razor component with explicitly provided data in the form of a dictionary.<br/>This method is intended for rendering components without a layout, suitable for inclusion in other views. |
| [View&lt;TComponent&gt;(object)](Rizzy.RizzyService.View_TComponent_(object).md 'Rizzy.RizzyService.View<TComponent>(object)') | Renders a view using the specified Razor component, optionally accepting dynamic data to pass to the component. |
| [View&lt;TComponent&gt;(Dictionary&lt;string,object&gt;)](Rizzy.RizzyService.View_TComponent_(System.Collections.Generic.Dictionary_string,object_).md 'Rizzy.RizzyService.View<TComponent>(System.Collections.Generic.Dictionary<string,object>)') | Renders a view using the specified Razor component with explicitly provided data in the form of a dictionary. |
