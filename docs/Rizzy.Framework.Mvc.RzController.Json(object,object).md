#### [Rizzy](index 'index')
### [Rizzy.Framework.Mvc](Rizzy.Framework.Mvc 'Rizzy.Framework.Mvc').[RzController](Rizzy.Framework.Mvc.RzController 'Rizzy.Framework.Mvc.RzController')

## RzController.Json(object, object) Method

Creates a [Microsoft.AspNetCore.Mvc.JsonResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.JsonResult 'Microsoft.AspNetCore.Mvc.JsonResult') object that serializes the specified [data](Rizzy.Framework.Mvc.RzController.Json(object,object)#Rizzy.Framework.Mvc.RzController.Json(object,object).data 'Rizzy.Framework.Mvc.RzController.Json(object, object).data') object  
to JSON.

```csharp
public virtual Microsoft.AspNetCore.Mvc.JsonResult Json(object? data, object? serializerSettings);
```
#### Parameters

<a name='Rizzy.Framework.Mvc.RzController.Json(object,object).data'></a>

`data` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

The object to serialize.

<a name='Rizzy.Framework.Mvc.RzController.Json(object,object).serializerSettings'></a>

`serializerSettings` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

The serializer settings to be used by the formatter.  
              
  
When using `System.Text.Json`, this should be an instance of [System.Text.Json.JsonSerializerOptions](https://docs.microsoft.com/en-us/dotnet/api/System.Text.Json.JsonSerializerOptions 'System.Text.Json.JsonSerializerOptions').  
  
When using `Newtonsoft.Json`, this should be an instance of `JsonSerializerSettings`.

#### Returns
[Microsoft.AspNetCore.Mvc.JsonResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.JsonResult 'Microsoft.AspNetCore.Mvc.JsonResult')  
The created [Microsoft.AspNetCore.Mvc.JsonResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.JsonResult 'Microsoft.AspNetCore.Mvc.JsonResult') that serializes the specified [data](Rizzy.Framework.Mvc.RzController.Json(object,object)#Rizzy.Framework.Mvc.RzController.Json(object,object).data 'Rizzy.Framework.Mvc.RzController.Json(object, object).data')  
            as JSON format for the response.

### Remarks
Callers should cache an instance of serializer settings to avoid  
            recreating cached data with each call.