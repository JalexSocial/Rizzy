#### [Rizzy](index 'index')
### [Rizzy.Nonce](Rizzy.Nonce 'Rizzy.Nonce').[RizzyNonceProvider](Rizzy.Nonce.RizzyNonceProvider 'Rizzy.Nonce.RizzyNonceProvider')

## RizzyNonceProvider.GetNonceValues() Method

Retrieves the nonce values for the current HTTP request.   
New nonce values are generated and cached if not already present.

```csharp
public Rizzy.Nonce.RizzyNonceValues GetNonceValues();
```

Implements [GetNonceValues()](Rizzy.Nonce.IRizzyNonceProvider.GetNonceValues() 'Rizzy.Nonce.IRizzyNonceProvider.GetNonceValues()')

#### Returns
[RizzyNonceValues](Rizzy.Nonce.RizzyNonceValues 'Rizzy.Nonce.RizzyNonceValues')  
An instance of [RizzyNonceValues](Rizzy.Nonce.RizzyNonceValues 'Rizzy.Nonce.RizzyNonceValues') containing nonce mappings for various nonce types.