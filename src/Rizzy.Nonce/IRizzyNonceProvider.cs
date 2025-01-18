namespace Rizzy.Nonce;

/// <summary>
/// Service interface for providing CSP nonce values on a scoped basis.
/// </summary>
public interface IRizzyNonceProvider
{
    /// <summary>
    /// Retrieves the nonce values for inline scripts, styles, and other types.
    /// If nonce values have already been generated for the current HTTP request, they are retrieved from the cache; otherwise, new values are generated, cached, and returned.
    /// </summary>
    /// <returns>An instance of <see cref="RizzyNonceValues"/> containing nonce values for various types.</returns>
    RizzyNonceValues GetNonceValues();

    /// <summary>
    /// Retrieves (or generates) the nonce value for a given nonce type.
    /// </summary>
    /// <param name="nonceType">The type of nonce required.</param>
    /// <returns>A string representing the nonce for the requested type.</returns>
    string GetNonceFor(NonceType nonceType);
}
