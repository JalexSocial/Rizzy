#### [Rizzy](index 'index')
### [Rizzy.Configuration](Rizzy.Configuration 'Rizzy.Configuration').[RizzyConfigBuilder](Rizzy.Configuration.RizzyConfigBuilder 'Rizzy.Configuration.RizzyConfigBuilder')

## RizzyConfigBuilder.WithHtmxConfiguration(string, Action<HtmxConfig>) Method

Configures a named instance of [HtmxConfig](Rizzy.HtmxConfig 'Rizzy.HtmxConfig') options.

```csharp
public Rizzy.Configuration.RizzyConfigBuilder WithHtmxConfiguration(string name, System.Action<Rizzy.HtmxConfig> configBuilder);
```
#### Parameters

<a name='Rizzy.Configuration.RizzyConfigBuilder.WithHtmxConfiguration(string,System.Action_Rizzy.HtmxConfig_).name'></a>

`name` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The name for the options instance.

<a name='Rizzy.Configuration.RizzyConfigBuilder.WithHtmxConfiguration(string,System.Action_Rizzy.HtmxConfig_).configBuilder'></a>

`configBuilder` [System.Action&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-1 'System.Action`1')[HtmxConfig](Rizzy.HtmxConfig 'Rizzy.HtmxConfig')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-1 'System.Action`1')

An action to configure the [HtmxConfig](Rizzy.HtmxConfig 'Rizzy.HtmxConfig') options.

#### Returns
[RizzyConfigBuilder](Rizzy.Configuration.RizzyConfigBuilder 'Rizzy.Configuration.RizzyConfigBuilder')  
The current [RizzyConfigBuilder](Rizzy.Configuration.RizzyConfigBuilder 'Rizzy.Configuration.RizzyConfigBuilder') instance.