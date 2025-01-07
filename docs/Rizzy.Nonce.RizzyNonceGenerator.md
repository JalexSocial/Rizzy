#### [Rizzy](index 'index')
### [Rizzy.Nonce](Rizzy.Nonce 'Rizzy.Nonce')

## RizzyNonceGenerator Class

Responsible for generating tamper-resistant nonce values [NonceUtil](Rizzy.Nonce.NonceUtil 'Rizzy.Nonce.NonceUtil')

```csharp
public sealed class RizzyNonceGenerator
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; RizzyNonceGenerator

| Constructors | |
| :--- | :--- |
| [RizzyNonceGenerator(IOptions&lt;RizzyConfig&gt;)](Rizzy.Nonce.RizzyNonceGenerator.RizzyNonceGenerator(Microsoft.Extensions.Options.IOptions_Rizzy.Configuration.RizzyConfig_) 'Rizzy.Nonce.RizzyNonceGenerator.RizzyNonceGenerator(Microsoft.Extensions.Options.IOptions<Rizzy.Configuration.RizzyConfig>)') | Construct a nonce with an optional pre-existing base-64 encoded HMAC key |

| Methods | |
| :--- | :--- |
| [CreateNonce()](Rizzy.Nonce.RizzyNonceGenerator.CreateNonce() 'Rizzy.Nonce.RizzyNonceGenerator.CreateNonce()') | Creates a tamper-resistant nonce. |
| [ValidateNonce(string)](Rizzy.Nonce.RizzyNonceGenerator.ValidateNonce(string) 'Rizzy.Nonce.RizzyNonceGenerator.ValidateNonce(string)') | Validates a tamper-resistant nonce. |
