#### [Rizzy](index 'index')
### [Rizzy.Nonce](Rizzy.Nonce 'Rizzy.Nonce').[RizzyNonceProvider](Rizzy.Nonce.RizzyNonceProvider 'Rizzy.Nonce.RizzyNonceProvider')

## RizzyNonceProvider(IHttpContextAccessor, RizzyNonceGenerator) Constructor

Initializes a new instance of the [RizzyNonceProvider](Rizzy.Nonce.RizzyNonceProvider 'Rizzy.Nonce.RizzyNonceProvider') class.

```csharp
public RizzyNonceProvider(Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor, Rizzy.Nonce.RizzyNonceGenerator generator);
```
#### Parameters

<a name='Rizzy.Nonce.RizzyNonceProvider.RizzyNonceProvider(Microsoft.AspNetCore.Http.IHttpContextAccessor,Rizzy.Nonce.RizzyNonceGenerator).httpContextAccessor'></a>

`httpContextAccessor` [Microsoft.AspNetCore.Http.IHttpContextAccessor](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IHttpContextAccessor 'Microsoft.AspNetCore.Http.IHttpContextAccessor')

The [Microsoft.AspNetCore.Http.IHttpContextAccessor](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.IHttpContextAccessor 'Microsoft.AspNetCore.Http.IHttpContextAccessor') to access the current HTTP context.

<a name='Rizzy.Nonce.RizzyNonceProvider.RizzyNonceProvider(Microsoft.AspNetCore.Http.IHttpContextAccessor,Rizzy.Nonce.RizzyNonceGenerator).generator'></a>

`generator` [RizzyNonceGenerator](Rizzy.Nonce.RizzyNonceGenerator 'Rizzy.Nonce.RizzyNonceGenerator')

An instance of [RizzyNonceGenerator](Rizzy.Nonce.RizzyNonceGenerator 'Rizzy.Nonce.RizzyNonceGenerator') to generate nonce values.