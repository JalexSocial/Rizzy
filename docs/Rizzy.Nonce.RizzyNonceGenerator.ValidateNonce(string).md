#### [Rizzy](index 'index')
### [Rizzy.Nonce](Rizzy.Nonce 'Rizzy.Nonce').[RizzyNonceGenerator](Rizzy.Nonce.RizzyNonceGenerator 'Rizzy.Nonce.RizzyNonceGenerator')

## RizzyNonceGenerator.ValidateNonce(string) Method

Validates a tamper-resistant nonce.

```csharp
public bool ValidateNonce(string nonceToken);
```
#### Parameters

<a name='Rizzy.Nonce.RizzyNonceGenerator.ValidateNonce(string).nonceToken'></a>

`nonceToken` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The Base64-encoded nonce token to validate.

#### Returns
[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')  
True if valid; false otherwise.