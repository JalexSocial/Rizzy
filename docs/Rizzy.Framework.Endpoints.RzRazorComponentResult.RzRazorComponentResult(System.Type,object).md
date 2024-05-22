#### [Rizzy](index.md 'index')
### [Rizzy.Framework.Endpoints](Rizzy.Framework.Endpoints.md 'Rizzy.Framework.Endpoints').[RzRazorComponentResult](Rizzy.Framework.Endpoints.RzRazorComponentResult.md 'Rizzy.Framework.Endpoints.RzRazorComponentResult')

## RzRazorComponentResult(Type, object) Constructor

Constructs an instance of [RzRazorComponentResult](Rizzy.Framework.Endpoints.RzRazorComponentResult.md 'Rizzy.Framework.Endpoints.RzRazorComponentResult').

```csharp
public RzRazorComponentResult(System.Type componentType, object parameters);
```
#### Parameters

<a name='Rizzy.Framework.Endpoints.RzRazorComponentResult.RzRazorComponentResult(System.Type,object).componentType'></a>

`componentType` [System.Type](https://docs.microsoft.com/en-us/dotnet/api/System.Type 'System.Type')

The type of the component to render. This must implement [Microsoft.AspNetCore.Components.IComponent](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Components.IComponent 'Microsoft.AspNetCore.Components.IComponent').

<a name='Rizzy.Framework.Endpoints.RzRazorComponentResult.RzRazorComponentResult(System.Type,object).parameters'></a>

`parameters` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

Parameters for the component.