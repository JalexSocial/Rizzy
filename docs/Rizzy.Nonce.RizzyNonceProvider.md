#### [Rizzy](index 'index')
### [Rizzy.Nonce](Rizzy.Nonce 'Rizzy.Nonce')

## RizzyNonceProvider Class

Provides nonce values for various types (script, style, etc.) for use in Content Security Policies.

```csharp
public sealed class RizzyNonceProvider :
Rizzy.Nonce.IRizzyNonceProvider
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; RizzyNonceProvider

Implements [IRizzyNonceProvider](Rizzy.Nonce.IRizzyNonceProvider 'Rizzy.Nonce.IRizzyNonceProvider')

| Constructors | |
| :--- | :--- |
| [RizzyNonceProvider(IHttpContextAccessor, RizzyNonceGenerator)](Rizzy.Nonce.RizzyNonceProvider.RizzyNonceProvider(Microsoft.AspNetCore.Http.IHttpContextAccessor,Rizzy.Nonce.RizzyNonceGenerator) 'Rizzy.Nonce.RizzyNonceProvider.RizzyNonceProvider(Microsoft.AspNetCore.Http.IHttpContextAccessor, Rizzy.Nonce.RizzyNonceGenerator)') | Initializes a new instance of the [RizzyNonceProvider](Rizzy.Nonce.RizzyNonceProvider 'Rizzy.Nonce.RizzyNonceProvider') class. |

| Methods | |
| :--- | :--- |
| [GetNonceFor(NonceType)](Rizzy.Nonce.RizzyNonceProvider.GetNonceFor(Rizzy.Nonce.NonceType) 'Rizzy.Nonce.RizzyNonceProvider.GetNonceFor(Rizzy.Nonce.NonceType)') | Retrieves (or generates if necessary) the nonce value for the specified nonce type. |
| [GetNonceValues()](Rizzy.Nonce.RizzyNonceProvider.GetNonceValues() 'Rizzy.Nonce.RizzyNonceProvider.GetNonceValues()') | Retrieves the nonce values for the current HTTP request. <br/>New nonce values are generated and cached if not already present. |
