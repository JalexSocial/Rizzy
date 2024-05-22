#### [Rizzy](index 'index')
### [Rizzy.Framework.Mvc](Rizzy.Framework.Mvc 'Rizzy.Framework.Mvc').[RzController](Rizzy.Framework.Mvc.RzController 'Rizzy.Framework.Mvc.RzController')

## RzController.Json(object) Method

Creates a [Microsoft.AspNetCore.Mvc.JsonResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.JsonResult 'Microsoft.AspNetCore.Mvc.JsonResult') object that serializes the specified [data](Rizzy.Framework.Mvc.RzController.Json(object)#Rizzy.Framework.Mvc.RzController.Json(object).data 'Rizzy.Framework.Mvc.RzController.Json(object).data') object  
to JSON.

```csharp
public virtual Microsoft.AspNetCore.Mvc.JsonResult Json(object? data);
```
#### Parameters

<a name='Rizzy.Framework.Mvc.RzController.Json(object).data'></a>

`data` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

The object to serialize.

#### Returns
[Microsoft.AspNetCore.Mvc.JsonResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.JsonResult 'Microsoft.AspNetCore.Mvc.JsonResult')  
The created [Microsoft.AspNetCore.Mvc.JsonResult](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Mvc.JsonResult 'Microsoft.AspNetCore.Mvc.JsonResult') that serializes the specified [data](Rizzy.Framework.Mvc.RzController.Json(object)#Rizzy.Framework.Mvc.RzController.Json(object).data 'Rizzy.Framework.Mvc.RzController.Json(object).data')  
            to JSON format for the response.