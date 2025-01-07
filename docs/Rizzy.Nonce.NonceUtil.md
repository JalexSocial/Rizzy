#### [Rizzy](index 'index')
### [Rizzy.Nonce](Rizzy.Nonce 'Rizzy.Nonce')

## NonceUtil Class

Utility class to generate tamper-resistant nonce values that can be passed back to the  
server for reuse

```csharp
public sealed class NonceUtil
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; NonceUtil

| Methods | |
| :--- | :--- |
| [CreateNonce(byte[])](Rizzy.Nonce.NonceUtil.CreateNonce(byte[]) 'Rizzy.Nonce.NonceUtil.CreateNonce(byte[])') | Creates a tamper-resistant nonce consisting of:<br/>- 16 random bytes (128 bits) as the nonce payload<br/>- 16-byte HMAC signature<br/><br/>The result is ~44 characters in Base64 when encoded. |
| [ValidateNonce(string, byte[])](Rizzy.Nonce.NonceUtil.ValidateNonce(string,byte[]) 'Rizzy.Nonce.NonceUtil.ValidateNonce(string, byte[])') | Validates a nonce that was produced by [CreateNonce(byte[])](Rizzy.Nonce.NonceUtil.CreateNonce(byte[]) 'Rizzy.Nonce.NonceUtil.CreateNonce(byte[])').<br/>Ensures it has not been tampered with by recomputing the HMAC signature. |
