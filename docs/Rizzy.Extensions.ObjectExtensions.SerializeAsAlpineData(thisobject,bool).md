#### [Rizzy](index 'index')
### [Rizzy.Extensions](Rizzy.Extensions 'Rizzy.Extensions').[ObjectExtensions](Rizzy.Extensions.ObjectExtensions 'Rizzy.Extensions.ObjectExtensions')

## ObjectExtensions.SerializeAsAlpineData(this object, bool) Method

Serializes .NET objects for use with Alpine.js x-data attribute  
Credit to Alexander Zeitler for developing this approach. It requires Newtonsoft because System.Text.Json doesn't allow non-standard quote characters.

```csharp
public static string SerializeAsAlpineData(this object? value, bool ignoreNullValues=false);
```
#### Parameters

<a name='Rizzy.Extensions.ObjectExtensions.SerializeAsAlpineData(thisobject,bool).value'></a>

`value` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

<a name='Rizzy.Extensions.ObjectExtensions.SerializeAsAlpineData(thisobject,bool).ignoreNullValues'></a>

`ignoreNullValues` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

#### Returns
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

### See Also