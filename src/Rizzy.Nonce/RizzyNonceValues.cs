namespace Rizzy.Nonce;

/// <summary>
/// Holds nonce values mapped to the given nonce types.
/// </summary>
public sealed class RizzyNonceValues
{
    /// <summary>
    /// Gets the mapping of nonce type to nonce value.
    /// </summary>
    public Dictionary<NonceType, string> Nonces { get; } = new Dictionary<NonceType, string>();

    /// <summary>
    /// Gets the nonce value for the specified nonce type.
    /// Returns an empty string if the nonce is not found.
    /// </summary>
    /// <param name="nonceType">The nonce type.</param>
    /// <returns>The nonce value, or an empty string if not found.</returns>
    public string GetNonce(NonceType nonceType)
    {
        return Nonces.TryGetValue(nonceType, out string value) ? value : string.Empty;
    }

    /// <summary>
    /// Sets a nonce value for the specified nonce type.
    /// </summary>
    /// <param name="nonceType">The nonce type.</param>
    /// <param name="nonce">The nonce value.</param>
    public void SetNonce(NonceType nonceType, string nonce)
    {
        Nonces[nonceType] = nonce;
    }

    /// <summary>
    /// A default empty nonce mapping.
    /// </summary>
    public static RizzyNonceValues DefaultValue => new RizzyNonceValues();
}
