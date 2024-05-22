#### [Rizzy](index.md 'index')
### [Rizzy.Configuration.Htmx](Rizzy.Configuration.Htmx.md 'Rizzy.Configuration.Htmx').[HtmxConfig](Rizzy.Configuration.Htmx.HtmxConfig.md 'Rizzy.Configuration.Htmx.HtmxConfig')

## HtmxConfig.TriggerSpecsCache Property

defaults to [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null'), the cache to store evaluated trigger specifications into, improving parsing  
performance at the cost of more memory usage. You may define a simple object to use a never-clearing cache, or  
implement your own system using a proxy object

```csharp
public Rizzy.Configuration.Htmx.TriggerSpecificationCache? TriggerSpecsCache { get; set; }
```

#### Property Value
[Rizzy.Configuration.Htmx.TriggerSpecificationCache](https://docs.microsoft.com/en-us/dotnet/api/Rizzy.Configuration.Htmx.TriggerSpecificationCache 'Rizzy.Configuration.Htmx.TriggerSpecificationCache')