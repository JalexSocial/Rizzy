#### [Rizzy](index 'index')
### [Rizzy.Nonce](Rizzy.Nonce 'Rizzy.Nonce')

## IRizzyNonceProvider Interface

Service interface for providing CSP nonce values on a scoped basis.

```csharp
public interface IRizzyNonceProvider
```

Derived  
&#8627; [RizzyNonceProvider](Rizzy.Nonce.RizzyNonceProvider 'Rizzy.Nonce.RizzyNonceProvider')

| Methods | |
| :--- | :--- |
| [GetNonceFor(NonceType)](Rizzy.Nonce.IRizzyNonceProvider.GetNonceFor(Rizzy.Nonce.NonceType) 'Rizzy.Nonce.IRizzyNonceProvider.GetNonceFor(Rizzy.Nonce.NonceType)') | Retrieves (or generates) the nonce value for a given nonce type. |
| [GetNonceValues()](Rizzy.Nonce.IRizzyNonceProvider.GetNonceValues() 'Rizzy.Nonce.IRizzyNonceProvider.GetNonceValues()') | Retrieves the nonce values for inline scripts, styles, and other types.<br/>If nonce values have already been generated for the current HTTP request, they are retrieved from the cache; otherwise, new values are generated, cached, and returned. |
