#### [Rizzy](index.md 'index')
### [Rizzy.Framework.Endpoints](Rizzy.Framework.Endpoints.md 'Rizzy.Framework.Endpoints')

## RzRazorComponentResult Class

An [Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult') that renders a Razor Component.

```csharp
public class RzRazorComponentResult :
Microsoft.AspNetCore.Http.IResult,
Microsoft.AspNetCore.Http.IStatusCodeHttpResult,
Microsoft.AspNetCore.Http.IContentTypeHttpResult
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; RzRazorComponentResult

Derived  
&#8627; [RzRazorComponentResult&lt;TComponent&gt;](Rizzy.Framework.Endpoints.RzRazorComponentResult_TComponent_.md 'Rizzy.Framework.Endpoints.RzRazorComponentResult<TComponent>')

Implements [Microsoft.AspNetCore.Http.IResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult 'Microsoft.AspNetCore.Http.IResult'), [Microsoft.AspNetCore.Http.IStatusCodeHttpResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IStatusCodeHttpResult 'Microsoft.AspNetCore.Http.IStatusCodeHttpResult'), [Microsoft.AspNetCore.Http.IContentTypeHttpResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IContentTypeHttpResult 'Microsoft.AspNetCore.Http.IContentTypeHttpResult')

| Constructors | |
| :--- | :--- |
| [RzRazorComponentResult(Type)](Rizzy.Framework.Endpoints.RzRazorComponentResult.RzRazorComponentResult(System.Type).md 'Rizzy.Framework.Endpoints.RzRazorComponentResult.RzRazorComponentResult(System.Type)') | Constructs an instance of [Microsoft.AspNetCore.Http.HttpResults.RazorComponentResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.HttpResults.RazorComponentResult 'Microsoft.AspNetCore.Http.HttpResults.RazorComponentResult'). |
| [RzRazorComponentResult(Type, object)](Rizzy.Framework.Endpoints.RzRazorComponentResult.RzRazorComponentResult(System.Type,object).md 'Rizzy.Framework.Endpoints.RzRazorComponentResult.RzRazorComponentResult(System.Type, object)') | Constructs an instance of [RzRazorComponentResult](Rizzy.Framework.Endpoints.RzRazorComponentResult.md 'Rizzy.Framework.Endpoints.RzRazorComponentResult'). |
| [RzRazorComponentResult(Type, IReadOnlyDictionary&lt;string,object&gt;)](Rizzy.Framework.Endpoints.RzRazorComponentResult.RzRazorComponentResult(System.Type,System.Collections.Generic.IReadOnlyDictionary_string,object_).md 'Rizzy.Framework.Endpoints.RzRazorComponentResult.RzRazorComponentResult(System.Type, System.Collections.Generic.IReadOnlyDictionary<string,object>)') | Constructs an instance of [RzRazorComponentResult](Rizzy.Framework.Endpoints.RzRazorComponentResult.md 'Rizzy.Framework.Endpoints.RzRazorComponentResult'). |

| Properties | |
| :--- | :--- |
| [ComponentType](Rizzy.Framework.Endpoints.RzRazorComponentResult.ComponentType.md 'Rizzy.Framework.Endpoints.RzRazorComponentResult.ComponentType') | Gets the component type. |
| [ContentType](Rizzy.Framework.Endpoints.RzRazorComponentResult.ContentType.md 'Rizzy.Framework.Endpoints.RzRazorComponentResult.ContentType') | Gets or sets the Content-Type header for the response. |
| [Parameters](Rizzy.Framework.Endpoints.RzRazorComponentResult.Parameters.md 'Rizzy.Framework.Endpoints.RzRazorComponentResult.Parameters') | Gets the parameters for the component. |
| [PreventStreamingRendering](Rizzy.Framework.Endpoints.RzRazorComponentResult.PreventStreamingRendering.md 'Rizzy.Framework.Endpoints.RzRazorComponentResult.PreventStreamingRendering') | Gets or sets a flag to indicate whether streaming rendering should be prevented. If true, the renderer will<br/>wait for the component hierarchy to complete asynchronous tasks such as loading before supplying the HTML response.<br/>If false, streaming rendering will be determined by the components being rendered.<br/><br/>The default value is false. |
| [StatusCode](Rizzy.Framework.Endpoints.RzRazorComponentResult.StatusCode.md 'Rizzy.Framework.Endpoints.RzRazorComponentResult.StatusCode') | Gets or sets the HTTP status code. |

| Methods | |
| :--- | :--- |
| [ExecuteAsync(HttpContext)](Rizzy.Framework.Endpoints.RzRazorComponentResult.ExecuteAsync(Microsoft.AspNetCore.Http.HttpContext).md 'Rizzy.Framework.Endpoints.RzRazorComponentResult.ExecuteAsync(Microsoft.AspNetCore.Http.HttpContext)') | Processes this result in the given [httpContext](Rizzy.Framework.Endpoints.RzRazorComponentResult.ExecuteAsync(Microsoft.AspNetCore.Http.HttpContext).md#Rizzy.Framework.Endpoints.RzRazorComponentResult.ExecuteAsync(Microsoft.AspNetCore.Http.HttpContext).httpContext 'Rizzy.Framework.Endpoints.RzRazorComponentResult.ExecuteAsync(Microsoft.AspNetCore.Http.HttpContext).httpContext'). |
