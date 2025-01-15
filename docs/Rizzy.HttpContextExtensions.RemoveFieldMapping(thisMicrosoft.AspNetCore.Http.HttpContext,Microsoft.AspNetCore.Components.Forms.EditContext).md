#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[HttpContextExtensions](Rizzy.HttpContextExtensions 'Rizzy.HttpContextExtensions')

## HttpContextExtensions.RemoveFieldMapping(this HttpContext, EditContext) Method

Removes the field mapping entry for the given [Microsoft.AspNetCore.Components.Forms.EditContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Components.Forms.EditContext 'Microsoft.AspNetCore.Components.Forms.EditContext') if it exists.  
Uses [Microsoft.AspNetCore.Http.HttpContext.Items](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.HttpContext.Items 'Microsoft.AspNetCore.Http.HttpContext.Items') as the storage.

```csharp
public static void RemoveFieldMapping(this Microsoft.AspNetCore.Http.HttpContext context, Microsoft.AspNetCore.Components.Forms.EditContext editContext);
```
#### Parameters

<a name='Rizzy.HttpContextExtensions.RemoveFieldMapping(thisMicrosoft.AspNetCore.Http.HttpContext,Microsoft.AspNetCore.Components.Forms.EditContext).context'></a>

`context` [Microsoft.AspNetCore.Http.HttpContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.HttpContext 'Microsoft.AspNetCore.Http.HttpContext')

The current [Microsoft.AspNetCore.Http.HttpContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.HttpContext 'Microsoft.AspNetCore.Http.HttpContext').

<a name='Rizzy.HttpContextExtensions.RemoveFieldMapping(thisMicrosoft.AspNetCore.Http.HttpContext,Microsoft.AspNetCore.Components.Forms.EditContext).editContext'></a>

`editContext` [Microsoft.AspNetCore.Components.Forms.EditContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Components.Forms.EditContext 'Microsoft.AspNetCore.Components.Forms.EditContext')

The [Microsoft.AspNetCore.Components.Forms.EditContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Components.Forms.EditContext 'Microsoft.AspNetCore.Components.Forms.EditContext') used to identify the mapping.