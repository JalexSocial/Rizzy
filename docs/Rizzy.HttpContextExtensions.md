#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy')

## HttpContextExtensions Class

```csharp
public static class HttpContextExtensions
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; HttpContextExtensions

| Methods | |
| :--- | :--- |
| [GetOrAddFieldMapping(this HttpContext, EditContext)](Rizzy.HttpContextExtensions.GetOrAddFieldMapping(thisMicrosoft.AspNetCore.Http.HttpContext,Microsoft.AspNetCore.Components.Forms.EditContext) 'Rizzy.HttpContextExtensions.GetOrAddFieldMapping(this Microsoft.AspNetCore.Http.HttpContext, Microsoft.AspNetCore.Components.Forms.EditContext)') | Gets (or creates if missing) the field mapping for the specified [Microsoft.AspNetCore.Components.Forms.EditContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Components.Forms.EditContext 'Microsoft.AspNetCore.Components.Forms.EditContext').<br/>Uses [Microsoft.AspNetCore.Http.HttpContext.Items](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.HttpContext.Items 'Microsoft.AspNetCore.Http.HttpContext.Items') as the storage. |
| [Htmx(this HttpRequest)](Rizzy.HttpContextExtensions.Htmx(thisMicrosoft.AspNetCore.Http.HttpRequest) 'Rizzy.HttpContextExtensions.Htmx(this Microsoft.AspNetCore.Http.HttpRequest)') | Extension method for HttpRequest that creates (or returns a cached) HtmxRequest. |
| [Htmx(this HttpResponse)](Rizzy.HttpContextExtensions.Htmx(thisMicrosoft.AspNetCore.Http.HttpResponse) 'Rizzy.HttpContextExtensions.Htmx(this Microsoft.AspNetCore.Http.HttpResponse)') | Extension method for HttpResponse that creates a new HtmxResponse and returns it. |
| [Htmx(this HttpResponse, Action&lt;HtmxResponse&gt;)](Rizzy.HttpContextExtensions.Htmx(thisMicrosoft.AspNetCore.Http.HttpResponse,System.Action_Rizzy.Http.HtmxResponse_) 'Rizzy.HttpContextExtensions.Htmx(this Microsoft.AspNetCore.Http.HttpResponse, System.Action<Rizzy.Http.HtmxResponse>)') | Extension method for HttpResponse that creates a new HtmxResponse and invokes the provided action. |
| [IsHtmx(this HttpRequest)](Rizzy.HttpContextExtensions.IsHtmx(thisMicrosoft.AspNetCore.Http.HttpRequest) 'Rizzy.HttpContextExtensions.IsHtmx(this Microsoft.AspNetCore.Http.HttpRequest)') | Extension method to check if a request is an Htmx request |
| [RemoveFieldMapping(this HttpContext, EditContext)](Rizzy.HttpContextExtensions.RemoveFieldMapping(thisMicrosoft.AspNetCore.Http.HttpContext,Microsoft.AspNetCore.Components.Forms.EditContext) 'Rizzy.HttpContextExtensions.RemoveFieldMapping(this Microsoft.AspNetCore.Http.HttpContext, Microsoft.AspNetCore.Components.Forms.EditContext)') | Removes the field mapping entry for the given [Microsoft.AspNetCore.Components.Forms.EditContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Components.Forms.EditContext 'Microsoft.AspNetCore.Components.Forms.EditContext') if it exists.<br/>Uses [Microsoft.AspNetCore.Http.HttpContext.Items](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.HttpContext.Items 'Microsoft.AspNetCore.Http.HttpContext.Items') as the storage. |
