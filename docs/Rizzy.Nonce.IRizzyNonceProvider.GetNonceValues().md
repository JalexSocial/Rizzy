#### [Rizzy](index 'index')
### [Rizzy.Nonce](Rizzy.Nonce 'Rizzy.Nonce').[IRizzyNonceProvider](Rizzy.Nonce.IRizzyNonceProvider 'Rizzy.Nonce.IRizzyNonceProvider')

## IRizzyNonceProvider.GetNonceValues() Method

Retrieves the nonce values for inline scripts, styles, and other types.  
If nonce values have already been generated for the current HTTP request, they are retrieved from the cache; otherwise, new values are generated, cached, and returned.

```csharp
Rizzy.Nonce.RizzyNonceValues GetNonceValues();
```

#### Returns
[RizzyNonceValues](Rizzy.Nonce.RizzyNonceValues 'Rizzy.Nonce.RizzyNonceValues')  
An instance of [RizzyNonceValues](Rizzy.Nonce.RizzyNonceValues 'Rizzy.Nonce.RizzyNonceValues') containing nonce values for various types.