#### [Rizzy](index.md 'index')
### [Rizzy.Framework.Endpoints](Rizzy.Framework.Endpoints.md 'Rizzy.Framework.Endpoints').[RzRazorComponentResult](Rizzy.Framework.Endpoints.RzRazorComponentResult.md 'Rizzy.Framework.Endpoints.RzRazorComponentResult')

## RzRazorComponentResult.ExecuteAsync(HttpContext) Method

Processes this result in the given [httpContext](Rizzy.Framework.Endpoints.RzRazorComponentResult.ExecuteAsync(Microsoft.AspNetCore.Http.HttpContext).md#Rizzy.Framework.Endpoints.RzRazorComponentResult.ExecuteAsync(Microsoft.AspNetCore.Http.HttpContext).httpContext 'Rizzy.Framework.Endpoints.RzRazorComponentResult.ExecuteAsync(Microsoft.AspNetCore.Http.HttpContext).httpContext').

```csharp
public System.Threading.Tasks.Task ExecuteAsync(Microsoft.AspNetCore.Http.HttpContext httpContext);
```
#### Parameters

<a name='Rizzy.Framework.Endpoints.RzRazorComponentResult.ExecuteAsync(Microsoft.AspNetCore.Http.HttpContext).httpContext'></a>

`httpContext` [Microsoft.AspNetCore.Http.HttpContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.HttpContext 'Microsoft.AspNetCore.Http.HttpContext')

An [Microsoft.AspNetCore.Http.HttpContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.HttpContext 'Microsoft.AspNetCore.Http.HttpContext') associated with the current request.

Implements [ExecuteAsync(HttpContext)](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IResult.ExecuteAsync#Microsoft_AspNetCore_Http_IResult_ExecuteAsync_Microsoft_AspNetCore_Http_HttpContext_ 'Microsoft.AspNetCore.Http.IResult.ExecuteAsync(Microsoft.AspNetCore.Http.HttpContext)')

#### Returns
[System.Threading.Tasks.Task](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task 'System.Threading.Tasks.Task')  
A [System.Threading.Tasks.Task](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task 'System.Threading.Tasks.Task') which will complete when execution is completed.