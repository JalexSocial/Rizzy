#### [Rizzy](index 'index')
### [Rizzy.Nonce](Rizzy.Nonce 'Rizzy.Nonce')

## RizzyNonceProvider Class

Provides nonce values for inline scripts and styles to enhance security by preventing  
the execution of unauthorized scripts and styles in web applications.

```csharp
public sealed class RizzyNonceProvider :
Rizzy.Nonce.IRizzyNonceProvider
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; RizzyNonceProvider

Implements [IRizzyNonceProvider](Rizzy.Nonce.IRizzyNonceProvider 'Rizzy.Nonce.IRizzyNonceProvider')

| Constructors | |
| :--- | :--- |
| [RizzyNonceProvider(IHttpContextAccessor, RizzyNonceGenerator)](Rizzy.Nonce.RizzyNonceProvider.RizzyNonceProvider(Microsoft.AspNetCore.Http.IHttpContextAccessor,Rizzy.Nonce.RizzyNonceGenerator) 'Rizzy.Nonce.RizzyNonceProvider.RizzyNonceProvider(Microsoft.AspNetCore.Http.IHttpContextAccessor, Rizzy.Nonce.RizzyNonceGenerator)') | Initializes a new instance of the [RizzyNonceProvider](Rizzy.Nonce.RizzyNonceProvider 'Rizzy.Nonce.RizzyNonceProvider') class. |

| Properties | |
| :--- | :--- |
| [InlineScriptNonce](Rizzy.Nonce.RizzyNonceProvider.InlineScriptNonce 'Rizzy.Nonce.RizzyNonceProvider.InlineScriptNonce') | Gets the nonce value to be used for inline scripts in the current HTTP request. |
| [InlineStyleNonce](Rizzy.Nonce.RizzyNonceProvider.InlineStyleNonce 'Rizzy.Nonce.RizzyNonceProvider.InlineStyleNonce') | Gets the nonce value to be used for inline styles in the current HTTP request. |

| Methods | |
| :--- | :--- |
| [GetNonceValues()](Rizzy.Nonce.RizzyNonceProvider.GetNonceValues() 'Rizzy.Nonce.RizzyNonceProvider.GetNonceValues()') | Retrieves the nonce values for inline scripts and styles. If the nonce values<br/>have already been generated for the current HTTP request or provided via headers,<br/>they are returned from the cache; otherwise, new nonce values are generated, cached, and returned. |
