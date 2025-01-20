#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy')

## IRizzyService Interface

```csharp
public interface IRizzyService
```

Derived  
&#8627; [RzController](Rizzy.Framework.Mvc.RzController 'Rizzy.Framework.Mvc.RzController')  
&#8627; [RzControllerWithViews](Rizzy.Framework.Mvc.RzControllerWithViews 'Rizzy.Framework.Mvc.RzControllerWithViews')  
&#8627; [RizzyService](Rizzy.RizzyService 'Rizzy.RizzyService')

| Properties | |
| :--- | :--- |
| [CurrentActionUrl](Rizzy.IRizzyService.CurrentActionUrl 'Rizzy.IRizzyService.CurrentActionUrl') | Returns the current action method url as a possible Form callback url but may be overridden manually in any form handler method<br/>This value can be used inside of form Razor Component views |

| Methods | |
| :--- | :--- |
| [PartialView(RenderFragment)](Rizzy.IRizzyService.PartialView(Microsoft.AspNetCore.Components.RenderFragment) 'Rizzy.IRizzyService.PartialView(Microsoft.AspNetCore.Components.RenderFragment)') | Renders a Razor component without a layout from a RenderFragment |
| [PartialView&lt;TComponent&gt;(object)](Rizzy.IRizzyService.PartialView_TComponent_(object) 'Rizzy.IRizzyService.PartialView<TComponent>(object)') | Renders a partial view using the specified Razor component, optionally accepting dynamic data to pass to the component.<br/>This method is intended for rendering components without a layout, suitable for inclusion in other views. |
| [PartialView&lt;TComponent&gt;(Dictionary&lt;string,object&gt;)](Rizzy.IRizzyService.PartialView_TComponent_(System.Collections.Generic.Dictionary_string,object_) 'Rizzy.IRizzyService.PartialView<TComponent>(System.Collections.Generic.Dictionary<string,object>)') | Renders a Razor component without a layout |
| [View&lt;TComponent&gt;(object)](Rizzy.IRizzyService.View_TComponent_(object) 'Rizzy.IRizzyService.View<TComponent>(object)') | Renders a view using the specified Razor component, optionally accepting dynamic data to pass to the component. |
| [View&lt;TComponent&gt;(Dictionary&lt;string,object&gt;)](Rizzy.IRizzyService.View_TComponent_(System.Collections.Generic.Dictionary_string,object_) 'Rizzy.IRizzyService.View<TComponent>(System.Collections.Generic.Dictionary<string,object>)') | Renders a view using the specified Razor component with explicitly provided data in the form of a dictionary. |
