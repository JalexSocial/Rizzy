#### [Rizzy](index 'index')
### [Rizzy.Nonce](Rizzy.Nonce 'Rizzy.Nonce').[RizzyNonceProvider](Rizzy.Nonce.RizzyNonceProvider 'Rizzy.Nonce.RizzyNonceProvider')

## RizzyNonceProvider.GetNonceFor(NonceType) Method

Retrieves (or generates if necessary) the nonce value for the specified nonce type.

```csharp
public string GetNonceFor(Rizzy.Nonce.NonceType nonceType);
```
#### Parameters

<a name='Rizzy.Nonce.RizzyNonceProvider.GetNonceFor(Rizzy.Nonce.NonceType).nonceType'></a>

`nonceType` [NonceType](Rizzy.Nonce.NonceType 'Rizzy.Nonce.NonceType')

The type of nonce required (for example, Script or Style).

Implements [GetNonceFor(NonceType)](Rizzy.Nonce.IRizzyNonceProvider.GetNonceFor(Rizzy.Nonce.NonceType) 'Rizzy.Nonce.IRizzyNonceProvider.GetNonceFor(Rizzy.Nonce.NonceType)')

#### Returns
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The nonce string for the specified type.