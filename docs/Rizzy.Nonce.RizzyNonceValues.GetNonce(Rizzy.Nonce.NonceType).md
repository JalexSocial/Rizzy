#### [Rizzy](index 'index')
### [Rizzy.Nonce](Rizzy.Nonce 'Rizzy.Nonce').[RizzyNonceValues](Rizzy.Nonce.RizzyNonceValues 'Rizzy.Nonce.RizzyNonceValues')

## RizzyNonceValues.GetNonce(NonceType) Method

Gets the nonce value for the specified nonce type.  
Returns an empty string if the nonce is not found.

```csharp
public string GetNonce(Rizzy.Nonce.NonceType nonceType);
```
#### Parameters

<a name='Rizzy.Nonce.RizzyNonceValues.GetNonce(Rizzy.Nonce.NonceType).nonceType'></a>

`nonceType` [NonceType](Rizzy.Nonce.NonceType 'Rizzy.Nonce.NonceType')

The nonce type.

#### Returns
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The nonce value, or an empty string if not found.