#### [Rizzy](index 'index')
### [Rizzy.Extensions](Rizzy.Extensions 'Rizzy.Extensions')

## ObjectExtensions Class

```csharp
public static class ObjectExtensions
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; ObjectExtensions

| Methods | |
| :--- | :--- |
| [SerializeAsAlpineData(this object, bool)](Rizzy.Extensions.ObjectExtensions.SerializeAsAlpineData(thisobject,bool) 'Rizzy.Extensions.ObjectExtensions.SerializeAsAlpineData(this object, bool)') | Serializes .NET objects for use with Alpine.js x-data attribute<br/>Credit to Alexander Zeitler for developing this approach. It requires Newtonsoft because System.Text.Json doesn't allow non-standard quote characters. |
| [ToDictionary(this object)](Rizzy.Extensions.ObjectExtensions.ToDictionary(thisobject) 'Rizzy.Extensions.ObjectExtensions.ToDictionary(this object)') | Converts a POCO object to a dictionary where each key/value is a property from the original POCO object |
