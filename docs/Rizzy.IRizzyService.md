#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy')

## IRizzyService Interface

```csharp
public interface IRizzyService
```

Derived  
&#8627; [RzController](Rizzy.Framework.Mvc.RzController.md 'Rizzy.Framework.Mvc.RzController')  
&#8627; [RzControllerWithViews](Rizzy.Framework.Mvc.RzControllerWithViews.md 'Rizzy.Framework.Mvc.RzControllerWithViews')  
&#8627; [RizzyService](Rizzy.RizzyService.md 'Rizzy.RizzyService')

| Properties | |
| :--- | :--- |
| [CurrentActionUrl](Rizzy.IRizzyService.CurrentActionUrl.md 'Rizzy.IRizzyService.CurrentActionUrl') | Returns the current action method url as a possible Form callback url but may be overridden manually in any form handler method<br/>This value can be used inside of form Razor Component views |
| [ViewContext](Rizzy.IRizzyService.ViewContext.md 'Rizzy.IRizzyService.ViewContext') | Gets the view context associated with the service. The view context contains information required for configuring and rendering views. |

| Methods | |
| :--- | :--- |
| [PartialView&lt;TComponent&gt;(object)](Rizzy.IRizzyService.PartialView_TComponent_(object).md 'Rizzy.IRizzyService.PartialView<TComponent>(object)') | Renders a partial view using the specified Razor component, optionally accepting dynamic data to pass to the component.<br/>This method is intended for rendering components without a layout, suitable for inclusion in other views. |
| [PartialView&lt;TComponent&gt;(Dictionary&lt;string,object&gt;)](Rizzy.IRizzyService.PartialView_TComponent_(System.Collections.Generic.Dictionary_string,object_).md 'Rizzy.IRizzyService.PartialView<TComponent>(System.Collections.Generic.Dictionary<string,object>)') | Renders a Razor component without a layout |
| [View&lt;TComponent&gt;(object)](Rizzy.IRizzyService.View_TComponent_(object).md 'Rizzy.IRizzyService.View<TComponent>(object)') | Renders a view using the specified Razor component, optionally accepting dynamic data to pass to the component. |
| [View&lt;TComponent&gt;(Dictionary&lt;string,object&gt;)](Rizzy.IRizzyService.View_TComponent_(System.Collections.Generic.Dictionary_string,object_).md 'Rizzy.IRizzyService.View<TComponent>(System.Collections.Generic.Dictionary<string,object>)') | Renders a view using the specified Razor component with explicitly provided data in the form of a dictionary. |
