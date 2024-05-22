#### [Rizzy](index 'index')
### [Rizzy.Framework.Endpoints](Rizzy.Framework.Endpoints 'Rizzy.Framework.Endpoints').[RzRazorComponentResult](Rizzy.Framework.Endpoints.RzRazorComponentResult 'Rizzy.Framework.Endpoints.RzRazorComponentResult')

## RzRazorComponentResult(Type, IReadOnlyDictionary<string,object>) Constructor

Constructs an instance of [RzRazorComponentResult](Rizzy.Framework.Endpoints.RzRazorComponentResult 'Rizzy.Framework.Endpoints.RzRazorComponentResult').

```csharp
public RzRazorComponentResult(System.Type componentType, System.Collections.Generic.IReadOnlyDictionary<string,object?> parameters);
```
#### Parameters

<a name='Rizzy.Framework.Endpoints.RzRazorComponentResult.RzRazorComponentResult(System.Type,System.Collections.Generic.IReadOnlyDictionary_string,object_).componentType'></a>

`componentType` [System.Type](https://docs.microsoft.com/en-us/dotnet/api/System.Type 'System.Type')

The type of the component to render. This must implement [Microsoft.AspNetCore.Components.IComponent](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Components.IComponent 'Microsoft.AspNetCore.Components.IComponent').

<a name='Rizzy.Framework.Endpoints.RzRazorComponentResult.RzRazorComponentResult(System.Type,System.Collections.Generic.IReadOnlyDictionary_string,object_).parameters'></a>

`parameters` [System.Collections.Generic.IReadOnlyDictionary&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyDictionary-2 'System.Collections.Generic.IReadOnlyDictionary`2')[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyDictionary-2 'System.Collections.Generic.IReadOnlyDictionary`2')[System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyDictionary-2 'System.Collections.Generic.IReadOnlyDictionary`2')

Parameters for the component.