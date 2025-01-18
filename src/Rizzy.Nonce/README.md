# Rizzy.Nonce

**Rizzy.Nonce** is a lightweight .NET library for generating and validating cryptographic nonces in ASP.NET Core and Blazor applications. This package is especially useful for enforcing Content Security Policies (CSP) by providing unique nonce values that can be attached to scripts and styles. These nonces help ensure that only trusted resources are allowed to execute, even when using external libraries like Alpine.js and Tailwind CSS.

## Installation

Install the package from NuGet using the package ID **Rizzy.Nonce**.

### Package Manager Console

```powershell
Install-Package Rizzy.Nonce
```

### .NET CLI

```bash
dotnet add package Rizzy.Nonce
```

## Secure HMAC Key Generation

You can generate a secure HMAC key (for example, 256âbit) with code like the following:

```csharp
/// <summary>
/// Generates a secure HMAC key using a cryptographically secure random number generator.
/// </summary>
/// <param name="keySizeInBytes">Size of the key in bytes (e.g., 32 for 256-bit).</param>
/// <returns>Base64-encoded HMAC key.</returns>
private byte[] GenerateSecureHmacKey(int keySizeInBytes = 32)
{
    byte[] key = new byte[keySizeInBytes];
    RandomNumberGenerator.Fill(key);
    return key;
}

private string GetBase64Key() => Convert.ToBase64String(GenerateSecureHmacKey());
```

Use the generated Base64 key to configure the nonce settings.

## Service Registration

When using minimal hosting (Program.cs), you register the nonce provider in your DI container using the provided extension method. This extension registers the `IRizzyNonceProvider` implementation and configures the dedicated `NonceOptions`. For example:

```csharp
using Rizzy.Nonce;
using Rizzy.Nonce.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register Rizzy.Nonce services and configure the HMAC key.
builder.Services.AddRizzyNonceProvider(options =>
{
    // Configure the HMAC key using your configuration
    options.HMACKey = builder.Configuration["Rizzy:NonceHMACKey"] ?? "";
});

```

This code ensures that the nonce provider is available for injection across your application. If you use configuration providers that support live reload (for example, JSON files), using `IOptionsMonitor<NonceOptions>` internally can help keep the nonce settings in sync with any changes.

## Using Nonces in a Blazor Application with a Content Security Policy

Below is an example of how you might use the nonce in your Blazor application's root layout. In this example, the nonces are injected into your HTML markup to allow externally loaded scripts and styles (such as Alpine.js and the Tailwind CDN) to pass a strict CSP.

```razor
@using Rizzy.Nonce
@inject IRizzyNonceProvider NonceProvider

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <!-- Content Security Policy Example -->
    <!-- This CSP allows scripts and styles only when they have the correct nonce -->
    <meta http-equiv="Content-Security-Policy" content="
        default-src 'self';
        script-src 'self' https://unpkg.com/alpinejs@3.x.x/dist/cdn.min.js 'nonce-@NonceProvider.GetNonceFor(NonceType.Script)';
        style-src 'self' https://cdn.jsdelivr.net/npm/tailwindcss@^2/dist/tailwind.min.css 'nonce-@NonceProvider.GetNonceFor(NonceType.Style)';
    " />

    <!-- External scripts/styles with nonce attributes -->
    <script src="https://unpkg.com/alpinejs@3.x.x/dist/cdn.min.js" nonce="@NonceProvider.GetNonceFor(NonceType.Script)"></script>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/tailwindcss@^2/dist/tailwind.min.css" nonce="@NonceProvider.GetNonceFor(NonceType.Style)" />

    <title>My Blazor App</title>
</head>
<body>
    <!-- Routes here -->
</body>
</html>
```

### Explanation

- **CSP Meta Tag:**  
  The Content Security Policy meta tag limits sources for scripts and styles. The CSP requires the external resources to include a nonce matching the generated value.
  
- **Nonce Attributes:**  
  External scripts and styles include a `nonce` attribute whose value is obtained via `NonceProvider.GetNonceFor(NonceType.Script)` and `NonceProvider.GetNonceFor(NonceType.Style)`, ensuring they comply with the CSP.

- **Blazor App Rendering:**  
  In a Blazor application, the `<app>` element is rendered using server prerendering, and nonces can also be propagated to other components as needed.

---

## Conclusion

**Rizzy.Nonce** simplifies secure nonce generation and validation for ASP.NET Core and Blazor applications. By using dedicated nonce options and registering via the minimal hosting approach in Program.cs, you ensure that your security policies remain modular, maintainable, and in sync with your overall application configuration.

If you have any questions or need further customization, please refer to the documentation or file an issue.
