#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy')

## RzViewContext Class

Represents the context for a view within an application, providing access to HTTP contexts, URL helpers, and component configurations.

```csharp
public class RzViewContext
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; RzViewContext

| Constructors | |
| :--- | :--- |
| [RzViewContext(IHttpContextAccessor, IUrlHelper)](Rizzy.RzViewContext.RzViewContext(Microsoft.AspNetCore.Http.IHttpContextAccessor,Microsoft.AspNetCore.Mvc.IUrlHelper).md 'Rizzy.RzViewContext.RzViewContext(Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Mvc.IUrlHelper)') | Represents the context for a view within an application, providing access to HTTP contexts, URL helpers, and component configurations. |

| Properties | |
| :--- | :--- |
| [ComponentParameters](Rizzy.RzViewContext.ComponentParameters.md 'Rizzy.RzViewContext.ComponentParameters') | This is a full list of all the parameters that are set on the component view |
| [Htmx](Rizzy.RzViewContext.Htmx.md 'Rizzy.RzViewContext.Htmx') | Gets the Htmx context for the current request. |
| [HttpContext](Rizzy.RzViewContext.HttpContext.md 'Rizzy.RzViewContext.HttpContext') | Gets or sets the [Microsoft.AspNetCore.Http.HttpContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.HttpContext 'Microsoft.AspNetCore.Http.HttpContext') for the current request. |
| [PageTitle](Rizzy.RzViewContext.PageTitle.md 'Rizzy.RzViewContext.PageTitle') | Sets the current page title |
| [RouteData](Rizzy.RzViewContext.RouteData.md 'Rizzy.RzViewContext.RouteData') | Gets or sets the AspNetCore.Routing.RouteData for the current request. |
| [Url](Rizzy.RzViewContext.Url.md 'Rizzy.RzViewContext.Url') | Provides access to the MVC UrlHelper which contains methods to build URLs for ASP.NET MVC within an application. |

| Methods | |
| :--- | :--- |
| [AddFormContext(string, object, bool)](Rizzy.RzViewContext.AddFormContext(string,object,bool).md 'Rizzy.RzViewContext.AddFormContext(string, object, bool)') | Attempts to add a form context with the specified name and model. |
| [AddFormContext(string, string, object, bool)](Rizzy.RzViewContext.AddFormContext(string,string,object,bool).md 'Rizzy.RzViewContext.AddFormContext(string, string, object, bool)') | Attempts to add a form context with the specified name and model. |
| [AddFormContext(string, string, string, object, bool)](Rizzy.RzViewContext.AddFormContext(string,string,string,object,bool).md 'Rizzy.RzViewContext.AddFormContext(string, string, string, object, bool)') | Attempts to add a form context with the specified name and model. |
| [TryGetFormContext(string, RzFormContext)](Rizzy.RzViewContext.TryGetFormContext(string,Rizzy.Components.Form.RzFormContext).md 'Rizzy.RzViewContext.TryGetFormContext(string, Rizzy.Components.Form.RzFormContext)') | Attempts to get a form context by name. |
