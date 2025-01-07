#### [Rizzy](index 'index')
### [Rizzy.Nonce](Rizzy.Nonce 'Rizzy.Nonce').[NonceUtil](Rizzy.Nonce.NonceUtil 'Rizzy.Nonce.NonceUtil')

## NonceUtil.ValidateNonce(string, byte[]) Method

Validates a nonce that was produced by [CreateNonce(byte[])](Rizzy.Nonce.NonceUtil.CreateNonce(byte[]) 'Rizzy.Nonce.NonceUtil.CreateNonce(byte[])').  
Ensures it has not been tampered with by recomputing the HMAC signature.

```csharp
public static bool ValidateNonce(string nonceToken, byte[] hmacKey);
```
#### Parameters

<a name='Rizzy.Nonce.NonceUtil.ValidateNonce(string,byte[]).nonceToken'></a>

`nonceToken` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The Base64-encoded token (~44 chars).

<a name='Rizzy.Nonce.NonceUtil.ValidateNonce(string,byte[]).hmacKey'></a>

`hmacKey` [System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

Same secret key used in [CreateNonce(byte[])](Rizzy.Nonce.NonceUtil.CreateNonce(byte[]) 'Rizzy.Nonce.NonceUtil.CreateNonce(byte[])').

#### Returns
[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')  
True if valid; false otherwise.