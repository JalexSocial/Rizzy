#### [Rizzy](index 'index')
### [Rizzy.Configuration](Rizzy.Configuration 'Rizzy.Configuration')

## RizzyConfigBuilder Class

Builds and registers the necessary Rizzy services and options.

```csharp
public class RizzyConfigBuilder
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; RizzyConfigBuilder

| Constructors | |
| :--- | :--- |
| [RizzyConfigBuilder(IServiceCollection, Action&lt;RizzyConfig&gt;, IConfiguration)](Rizzy.Configuration.RizzyConfigBuilder.RizzyConfigBuilder(Microsoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Rizzy.Configuration.RizzyConfig_,Microsoft.Extensions.Configuration.IConfiguration) 'Rizzy.Configuration.RizzyConfigBuilder.RizzyConfigBuilder(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Action<Rizzy.Configuration.RizzyConfig>, Microsoft.Extensions.Configuration.IConfiguration)') | Initializes a new instance of the [RizzyConfigBuilder](Rizzy.Configuration.RizzyConfigBuilder 'Rizzy.Configuration.RizzyConfigBuilder') class. |

| Methods | |
| :--- | :--- |
| [WithHtmxConfiguration(string, Action&lt;HtmxConfig&gt;)](Rizzy.Configuration.RizzyConfigBuilder.WithHtmxConfiguration(string,System.Action_Rizzy.HtmxConfig_) 'Rizzy.Configuration.RizzyConfigBuilder.WithHtmxConfiguration(string, System.Action<Rizzy.HtmxConfig>)') | Configures a named instance of [HtmxConfig](Rizzy.HtmxConfig 'Rizzy.HtmxConfig') options. |
| [WithHtmxConfiguration(Action&lt;HtmxConfig&gt;)](Rizzy.Configuration.RizzyConfigBuilder.WithHtmxConfiguration(System.Action_Rizzy.HtmxConfig_) 'Rizzy.Configuration.RizzyConfigBuilder.WithHtmxConfiguration(System.Action<Rizzy.HtmxConfig>)') | Configures the [HtmxConfig](Rizzy.HtmxConfig 'Rizzy.HtmxConfig') options. |
