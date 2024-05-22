#### [Rizzy](index 'index')
### [Rizzy.Framework.Endpoints](Rizzy.Framework.Endpoints 'Rizzy.Framework.Endpoints').[RzRazorComponentResult](Rizzy.Framework.Endpoints.RzRazorComponentResult 'Rizzy.Framework.Endpoints.RzRazorComponentResult')

## RzRazorComponentResult(Type) Constructor

Constructs an instance of [Microsoft.AspNetCore.Http.HttpResults.RazorComponentResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.HttpResults.RazorComponentResult 'Microsoft.AspNetCore.Http.HttpResults.RazorComponentResult').

```csharp
public RzRazorComponentResult(System.Type componentType);
```
#### Parameters

<a name='Rizzy.Framework.Endpoints.RzRazorComponentResult.RzRazorComponentResult(System.Type).componentType'></a>

`componentType` [System.Type](https://docs.microsoft.com/en-us/dotnet/api/System.Type 'System.Type')

The type of the component to render. This must implement [Microsoft.AspNetCore.Components.IComponent](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Components.IComponent 'Microsoft.AspNetCore.Components.IComponent').