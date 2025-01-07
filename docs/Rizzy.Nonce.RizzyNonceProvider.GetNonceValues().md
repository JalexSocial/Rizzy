#### [Rizzy](index 'index')
### [Rizzy.Nonce](Rizzy.Nonce 'Rizzy.Nonce').[RizzyNonceProvider](Rizzy.Nonce.RizzyNonceProvider 'Rizzy.Nonce.RizzyNonceProvider')

## RizzyNonceProvider.GetNonceValues() Method

Retrieves the nonce values for inline scripts and styles. If the nonce values  
have already been generated for the current HTTP request or provided via headers,  
they are returned from the cache; otherwise, new nonce values are generated, cached, and returned.

```csharp
public Rizzy.Nonce.RizzyNonceValues GetNonceValues();
```

Implements [GetNonceValues()](Rizzy.Nonce.IRizzyNonceProvider.GetNonceValues() 'Rizzy.Nonce.IRizzyNonceProvider.GetNonceValues()')

#### Returns
[RizzyNonceValues](Rizzy.Nonce.RizzyNonceValues 'Rizzy.Nonce.RizzyNonceValues')  
An instance of [RizzyNonceValues](Rizzy.Nonce.RizzyNonceValues 'Rizzy.Nonce.RizzyNonceValues') containing the generated or reused nonce values.

#### Exceptions

[System.InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/System.InvalidOperationException 'System.InvalidOperationException')  
Thrown if there is no current [Microsoft.AspNetCore.Http.HttpContext](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.AspNetCore.Http.HttpContext 'Microsoft.AspNetCore.Http.HttpContext').