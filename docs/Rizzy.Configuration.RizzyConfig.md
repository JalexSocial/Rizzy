#### [Rizzy](index 'index')
### [Rizzy.Configuration](Rizzy.Configuration 'Rizzy.Configuration')

## RizzyConfig Class

```csharp
public class RizzyConfig
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; RizzyConfig

| Properties | |
| :--- | :--- |
| [DefaultLayout](Rizzy.Configuration.RizzyConfig.DefaultLayout 'Rizzy.Configuration.RizzyConfig.DefaultLayout') | Layout that is applied to all pages without a layout attribute |
| [NonceHMACKey](Rizzy.Configuration.RizzyConfig.NonceHMACKey 'Rizzy.Configuration.RizzyConfig.NonceHMACKey') | Gets or sets the Nonce HMAC Key as a base-64 encoded string. This key is used to sign generated nonce values<br/>such that they may be revalidated when submitted again via htmx |
| [NonceHMACKeyBytes](Rizzy.Configuration.RizzyConfig.NonceHMACKeyBytes 'Rizzy.Configuration.RizzyConfig.NonceHMACKeyBytes') | Gets the decoded Nonce HMAC Key as a byte array. |
| [RootComponent](Rizzy.Configuration.RizzyConfig.RootComponent 'Rizzy.Configuration.RizzyConfig.RootComponent') | Core application layout that contains full html page layout |
