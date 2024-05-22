#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[HtmxConfig](Rizzy.HtmxConfig.md 'Rizzy.HtmxConfig')

## HtmxConfig.RefreshOnHistoryMiss Property

Defaults to [false](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool') if this property is null.  
If set to [true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool') htmx will issue a full page refresh on history misses rather than use an AJAX request.

```csharp
public System.Nullable<bool> RefreshOnHistoryMiss { get; set; }
```

#### Property Value
[System.Nullable&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1 'System.Nullable`1')[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1 'System.Nullable`1')