#### [Rizzy](index.md 'index')
### [Rizzy](Rizzy.md 'Rizzy')

## TriggerSpecificationCache Class

htmx configuration allows for the creation of a trigger specification cache in order to improve  
trigger-handling performance.  The cache is a key/value store mapping well-formed hx-trigger parameters  
to their parsed specifications.

```csharp
public class TriggerSpecificationCache : System.Collections.Generic.Dictionary<string, System.Collections.Generic.IReadOnlyList<Rizzy.HtmxTriggerSpecification>>
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [System.Collections.Generic.Dictionary&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2')[System.Collections.Generic.IReadOnlyList&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyList-1 'System.Collections.Generic.IReadOnlyList`1')[HtmxTriggerSpecification](Rizzy.HtmxTriggerSpecification.md 'Rizzy.HtmxTriggerSpecification')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyList-1 'System.Collections.Generic.IReadOnlyList`1')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2 'System.Collections.Generic.Dictionary`2') &#129106; TriggerSpecificationCache