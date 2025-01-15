#### [Rizzy](index 'index')
### [Rizzy](Rizzy 'Rizzy').[HttpContextExtensions](Rizzy.HttpContextExtensions 'Rizzy.HttpContextExtensions')

## HttpContextExtensions.GetOrAddFieldMapping(this HttpContext, EditContext) Method

Gets (or creates if missing) the field mapping for the specified [Microsoft.AspNetCore.Components.Forms.EditContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Components.Forms.EditContext 'Microsoft.AspNetCore.Components.Forms.EditContext').  
Uses [Microsoft.AspNetCore.Http.HttpContext.Items](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.HttpContext.Items 'Microsoft.AspNetCore.Http.HttpContext.Items') as the storage.

```csharp
public static System.Collections.Generic.Dictionary<Microsoft.AspNetCore.Components.Forms.FieldIdentifier,Rizzy.RzFormFieldMap> GetOrAddFieldMapping(this Microsoft.AspNetCore.Http.HttpContext context, Microsoft.AspNetCore.Components.Forms.EditContext editContext);
```
#### Parameters

<a name='Rizzy.HttpContextExtensions.GetOrAddFieldMapping(thisMicrosoft.AspNetCore.Http.HttpContext,Microsoft.AspNetCore.Components.Forms.EditContext).context'></a>

`context` [Microsoft.AspNetCore.Http.HttpContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.HttpContext 'Microsoft.AspNetCore.Http.HttpContext')

The current [Microsoft.AspNetCore.Http.HttpContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.HttpContext 'Microsoft.AspNetCore.Http.HttpContext').

<a name='Rizzy.HttpContextExtensions.GetOrAddFieldMapping(thisMicrosoft.AspNetCore.Http.HttpContext,Microsoft.AspNetCore.Components.Forms.EditContext).editContext'></a>

`editContext` [Microsoft.AspNetCore.Components.Forms.EditContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Components.Forms.EditContext 'Microsoft.AspNetCore.Components.Forms.EditContext')

The [Microsoft.AspNetCore.Components.Forms.EditContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Components.Forms.EditContext 'Microsoft.AspNetCore.Components.Forms.EditContext') used to identify the mapping.

#### Returns
[System.Collections.Generic.Dictionary&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')[Microsoft.AspNetCore.Components.Forms.FieldIdentifier](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Components.Forms.FieldIdentifier 'Microsoft.AspNetCore.Components.Forms.FieldIdentifier')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')[RzFormFieldMap](Rizzy.RzFormFieldMap 'Rizzy.RzFormFieldMap')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')  
A [System.Collections.Generic.Dictionary&lt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2') for the given [Microsoft.AspNetCore.Components.Forms.EditContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Components.Forms.EditContext 'Microsoft.AspNetCore.Components.Forms.EditContext').