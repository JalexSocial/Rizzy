#### [Rizzy](index 'index')
### [Rizzy.Nonce](Rizzy.Nonce 'Rizzy.Nonce').[NonceUtil](Rizzy.Nonce.NonceUtil 'Rizzy.Nonce.NonceUtil')

## NonceUtil.CreateNonce(byte[]) Method

Creates a tamper-resistant nonce consisting of:  
- 16 random bytes (128 bits) as the nonce payload  
- 16-byte HMAC signature  
  
The result is ~44 characters in Base64 when encoded.

```csharp
public static string CreateNonce(byte[] hmacKey);
```
#### Parameters

<a name='Rizzy.Nonce.NonceUtil.CreateNonce(byte[]).hmacKey'></a>

`hmacKey` [System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

A secret key used for HMAC signing.

#### Returns
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
Base64-encoded token combining the random data and its signature.