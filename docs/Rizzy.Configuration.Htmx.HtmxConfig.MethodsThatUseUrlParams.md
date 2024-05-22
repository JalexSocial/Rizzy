#### [Rizzy](index.md 'index')
### [Rizzy.Configuration.Htmx](Rizzy.Configuration.Htmx.md 'Rizzy.Configuration.Htmx').[HtmxConfig](Rizzy.Configuration.Htmx.HtmxConfig.md 'Rizzy.Configuration.Htmx.HtmxConfig')

## HtmxConfig.MethodsThatUseUrlParams Property

Defaults to `["get"]` if this property is null.  
Htmx will format requests with these methods by encoding their parameters in the URL, not the request body.

```csharp
public string[]? MethodsThatUseUrlParams { get; set; }
```

#### Property Value
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')