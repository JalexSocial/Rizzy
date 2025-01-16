#### [Rizzy](index 'index')

## Rizzy.Nonce Namespace

| Classes | |
| :--- | :--- |
| [NonceUtil](Rizzy.Nonce.NonceUtil 'Rizzy.Nonce.NonceUtil') | Utility class to generate tamper-resistant nonce values that can be passed back to the<br/>server for reuse |
| [RizzyNonceGenerator](Rizzy.Nonce.RizzyNonceGenerator 'Rizzy.Nonce.RizzyNonceGenerator') | Responsible for generating tamper-resistant nonce values [NonceUtil](Rizzy.Nonce.NonceUtil 'Rizzy.Nonce.NonceUtil') |
| [RizzyNonceProvider](Rizzy.Nonce.RizzyNonceProvider 'Rizzy.Nonce.RizzyNonceProvider') | Provides nonce values for various types (script, style, etc.) for use in Content Security Policies. |
| [RizzyNonceValues](Rizzy.Nonce.RizzyNonceValues 'Rizzy.Nonce.RizzyNonceValues') | Holds nonce values mapped to the given nonce types. |

| Interfaces | |
| :--- | :--- |
| [IRizzyNonceProvider](Rizzy.Nonce.IRizzyNonceProvider 'Rizzy.Nonce.IRizzyNonceProvider') | Service interface for providing CSP nonce values on a scoped basis. |

| Enums | |
| :--- | :--- |
| [NonceType](Rizzy.Nonce.NonceType 'Rizzy.Nonce.NonceType') | Defines the different types of nonces used in Content Security Policies. |
