#### [Rizzy](index 'index')
### [Rizzy.Configuration](Rizzy.Configuration 'Rizzy.Configuration').[RizzyConfigBuilder](Rizzy.Configuration.RizzyConfigBuilder 'Rizzy.Configuration.RizzyConfigBuilder')

## RizzyConfigBuilder.ConfigureHtmxSettings Class

Class responsible for configuring [HtmxConfig](Rizzy.HtmxConfig 'Rizzy.HtmxConfig') options.

```csharp
public class RizzyConfigBuilder.ConfigureHtmxSettings :
Microsoft.Extensions.Options.IConfigureOptions<Rizzy.HtmxConfig>,
Microsoft.Extensions.Options.IConfigureNamedOptions<Rizzy.HtmxConfig>
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; ConfigureHtmxSettings

Implements [Microsoft.Extensions.Options.IConfigureOptions&lt;](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.Extensions.Options.IConfigureOptions-1 'Microsoft.Extensions.Options.IConfigureOptions`1')[HtmxConfig](Rizzy.HtmxConfig 'Rizzy.HtmxConfig')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.Extensions.Options.IConfigureOptions-1 'Microsoft.Extensions.Options.IConfigureOptions`1'), [Microsoft.Extensions.Options.IConfigureNamedOptions&lt;](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.Extensions.Options.IConfigureNamedOptions-1 'Microsoft.Extensions.Options.IConfigureNamedOptions`1')[HtmxConfig](Rizzy.HtmxConfig 'Rizzy.HtmxConfig')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.Extensions.Options.IConfigureNamedOptions-1 'Microsoft.Extensions.Options.IConfigureNamedOptions`1')

| Constructors | |
| :--- | :--- |
| [ConfigureHtmxSettings(IOptions&lt;HtmxAntiforgeryOptions&gt;)](Rizzy.Configuration.RizzyConfigBuilder.ConfigureHtmxSettings.ConfigureHtmxSettings(Microsoft.Extensions.Options.IOptions_Rizzy.Antiforgery.HtmxAntiforgeryOptions_) 'Rizzy.Configuration.RizzyConfigBuilder.ConfigureHtmxSettings.ConfigureHtmxSettings(Microsoft.Extensions.Options.IOptions<Rizzy.Antiforgery.HtmxAntiforgeryOptions>)') | Initializes a new instance of the [ConfigureHtmxSettings](Rizzy.Configuration.RizzyConfigBuilder.ConfigureHtmxSettings 'Rizzy.Configuration.RizzyConfigBuilder.ConfigureHtmxSettings') class. |

| Methods | |
| :--- | :--- |
| [Configure(HtmxConfig)](Rizzy.Configuration.RizzyConfigBuilder.ConfigureHtmxSettings.Configure(Rizzy.HtmxConfig) 'Rizzy.Configuration.RizzyConfigBuilder.ConfigureHtmxSettings.Configure(Rizzy.HtmxConfig)') | Configures the default [HtmxConfig](Rizzy.HtmxConfig 'Rizzy.HtmxConfig') options. |
| [Configure(string, HtmxConfig)](Rizzy.Configuration.RizzyConfigBuilder.ConfigureHtmxSettings.Configure(string,Rizzy.HtmxConfig) 'Rizzy.Configuration.RizzyConfigBuilder.ConfigureHtmxSettings.Configure(string, Rizzy.HtmxConfig)') | Configures a named instance of [HtmxConfig](Rizzy.HtmxConfig 'Rizzy.HtmxConfig') options. |
