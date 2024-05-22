#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy').[HtmxConfig](Rizzy.HtmxConfig.md 'Rizzy.HtmxConfig')

## HtmxConfig.TriggerSpecsCache Property

defaults to [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null'), the cache to store evaluated trigger specifications into, improving parsing  
performance at the cost of more memory usage. You may define a simple object to use a never-clearing cache, or  
implement your own system using a proxy object

```csharp
public Rizzy.TriggerSpecificationCache? TriggerSpecsCache { get; set; }
```

#### Property Value
[TriggerSpecificationCache](Rizzy.TriggerSpecificationCache.md 'Rizzy.TriggerSpecificationCache')