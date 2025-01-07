#### [Rizzy](index 'index')
### [Rizzy.Nonce](Rizzy.Nonce 'Rizzy.Nonce')

## IRizzyNonceProvider Interface

Service interface for providing CSP nonce values on a scoped basis.

```csharp
public interface IRizzyNonceProvider
```

Derived  
&#8627; [RizzyNonceProvider](Rizzy.Nonce.RizzyNonceProvider 'Rizzy.Nonce.RizzyNonceProvider')

| Properties | |
| :--- | :--- |
| [InlineScriptNonce](Rizzy.Nonce.IRizzyNonceProvider.InlineScriptNonce 'Rizzy.Nonce.IRizzyNonceProvider.InlineScriptNonce') | The nonce to add to any inlined scripts |
| [InlineStyleNonce](Rizzy.Nonce.IRizzyNonceProvider.InlineStyleNonce 'Rizzy.Nonce.IRizzyNonceProvider.InlineStyleNonce') | The nonce to add to any inlined styles |

| Methods | |
| :--- | :--- |
| [GetNonceValues()](Rizzy.Nonce.IRizzyNonceProvider.GetNonceValues() 'Rizzy.Nonce.IRizzyNonceProvider.GetNonceValues()') | Retrieves the nonce values for inline scripts and styles. If the nonce values<br/>have already been generated for the current HTTP request, they are returned<br/>from the cache; otherwise, new nonce values are generated, cached, and returned. |
