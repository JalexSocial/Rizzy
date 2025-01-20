#### [Rizzy](index 'index')
### [Rizzy.Configuration](Rizzy.Configuration 'Rizzy.Configuration').[RizzyConfigBuilder](Rizzy.Configuration.RizzyConfigBuilder 'Rizzy.Configuration.RizzyConfigBuilder')

## RizzyConfigBuilder(IServiceCollection, Action<RizzyConfig>, IConfiguration) Constructor

Initializes a new instance of the [RizzyConfigBuilder](Rizzy.Configuration.RizzyConfigBuilder 'Rizzy.Configuration.RizzyConfigBuilder') class.

```csharp
public RizzyConfigBuilder(Microsoft.Extensions.DependencyInjection.IServiceCollection services, System.Action<Rizzy.Configuration.RizzyConfig>? configBuilder=null, Microsoft.Extensions.Configuration.IConfiguration? configuration=null);
```
#### Parameters

<a name='Rizzy.Configuration.RizzyConfigBuilder.RizzyConfigBuilder(Microsoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Rizzy.Configuration.RizzyConfig_,Microsoft.Extensions.Configuration.IConfiguration).services'></a>

`services` [Microsoft.Extensions.DependencyInjection.IServiceCollection](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.Extensions.DependencyInjection.IServiceCollection 'Microsoft.Extensions.DependencyInjection.IServiceCollection')

The service collection to add services to.

<a name='Rizzy.Configuration.RizzyConfigBuilder.RizzyConfigBuilder(Microsoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Rizzy.Configuration.RizzyConfig_,Microsoft.Extensions.Configuration.IConfiguration).configBuilder'></a>

`configBuilder` [System.Action&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-1 'System.Action`1')[RizzyConfig](Rizzy.Configuration.RizzyConfig 'Rizzy.Configuration.RizzyConfig')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-1 'System.Action`1')

An optional action to configure the [RizzyConfig](Rizzy.Configuration.RizzyConfig 'Rizzy.Configuration.RizzyConfig').  
If not provided, defaults will be applied.

<a name='Rizzy.Configuration.RizzyConfigBuilder.RizzyConfigBuilder(Microsoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Rizzy.Configuration.RizzyConfig_,Microsoft.Extensions.Configuration.IConfiguration).configuration'></a>

`configuration` [Microsoft.Extensions.Configuration.IConfiguration](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.Extensions.Configuration.IConfiguration 'Microsoft.Extensions.Configuration.IConfiguration')

Optionally, an [Microsoft.Extensions.Configuration.IConfiguration](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.Extensions.Configuration.IConfiguration 'Microsoft.Extensions.Configuration.IConfiguration') instance to allow reading configuration values.

#### Exceptions

[System.ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentNullException 'System.ArgumentNullException')  
Thrown if [services](Rizzy.Configuration.RizzyConfigBuilder.RizzyConfigBuilder(Microsoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Rizzy.Configuration.RizzyConfig_,Microsoft.Extensions.Configuration.IConfiguration)#Rizzy.Configuration.RizzyConfigBuilder.RizzyConfigBuilder(Microsoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Rizzy.Configuration.RizzyConfig_,Microsoft.Extensions.Configuration.IConfiguration).services 'Rizzy.Configuration.RizzyConfigBuilder.RizzyConfigBuilder(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Action<Rizzy.Configuration.RizzyConfig>, Microsoft.Extensions.Configuration.IConfiguration).services') is `null`.