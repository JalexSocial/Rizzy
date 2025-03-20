using Microsoft.AspNetCore.Http;
using Rizzy.Htmx;
using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;

namespace Rizzy;

/// <summary>
/// Provides nonce values for various types (script, style, etc.) for use in Content Security Policies.
/// </summary>
public sealed class RizzyNonceProvider : IRizzyNonceProvider
{
    // HTTP context accessor to retrieve the current HTTP request context.
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="RizzyNonceProvider"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The <see cref="IHttpContextAccessor"/> to access the current HTTP context.</param>
    public RizzyNonceProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <summary>
    /// Retrieves (or generates) a nonce value for use with content security policies.
    /// </summary>
    /// <returns>A string representing a content security policy compatible nonce</returns>
    public string GetNonce()
    {
        var context = _httpContextAccessor.HttpContext
                      ?? throw new InvalidOperationException("No HttpContext available.");

        // If nonce values have already been generated for this request, return them.
        if (context.Items.TryGetValue(Constants.HttpContextKeys.NonceKey, out object? eNonce) && eNonce is string eNonceValue)
        {
            return eNonceValue;
        }

        // Create a new nonce values container.
        var nonce = CreateNonce();

        // Cache the nonce values for the current request.
        context.Items[Constants.HttpContextKeys.NonceKey] = nonce;

        return nonce;
    }

    /// <summary>
    /// Creates a base 64 encoded nonce string
    /// </summary>
    /// <returns></returns>
    private static string CreateNonce()
    {
        byte[] randomBytes = new byte[32];
        RandomNumberGenerator.Fill(randomBytes);

        return Base64UrlTextEncoder.Encode(randomBytes);
    }
}