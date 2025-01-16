#### [Rizzy](index 'index')
### [Rizzy.Nonce](Rizzy.Nonce 'Rizzy.Nonce').[IRizzyNonceProvider](Rizzy.Nonce.IRizzyNonceProvider 'Rizzy.Nonce.IRizzyNonceProvider')

## IRizzyNonceProvider.GetNonceFor(NonceType) Method

Retrieves (or generates) the nonce value for a given nonce type.

```csharp
string GetNonceFor(Rizzy.Nonce.NonceType nonceType);
```
#### Parameters

<a name='Rizzy.Nonce.IRizzyNonceProvider.GetNonceFor(Rizzy.Nonce.NonceType).nonceType'></a>

`nonceType` [NonceType](Rizzy.Nonce.NonceType 'Rizzy.Nonce.NonceType')

The type of nonce required.

#### Returns
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
A string representing the nonce for the requested type.