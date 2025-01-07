#### [Rizzy](index 'index')
### [Rizzy.Nonce](Rizzy.Nonce 'Rizzy.Nonce').[RizzyNonceGenerator](Rizzy.Nonce.RizzyNonceGenerator 'Rizzy.Nonce.RizzyNonceGenerator')

## RizzyNonceGenerator(IOptions<RizzyConfig>) Constructor

Construct a nonce with an optional pre-existing base-64 encoded HMAC key

```csharp
public RizzyNonceGenerator(Microsoft.Extensions.Options.IOptions<Rizzy.Configuration.RizzyConfig> options);
```
#### Parameters

<a name='Rizzy.Nonce.RizzyNonceGenerator.RizzyNonceGenerator(Microsoft.Extensions.Options.IOptions_Rizzy.Configuration.RizzyConfig_).options'></a>

`options` [Microsoft.Extensions.Options.IOptions&lt;](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.Extensions.Options.IOptions-1 'Microsoft.Extensions.Options.IOptions`1')[RizzyConfig](Rizzy.Configuration.RizzyConfig 'Rizzy.Configuration.RizzyConfig')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.Extensions.Options.IOptions-1 'Microsoft.Extensions.Options.IOptions`1')

Configuration options for RizzyUI

#### Exceptions

[System.ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentException 'System.ArgumentException')

### Remarks
HMAC key must be base-64 encoded and at least 256 bits (32 bytes)