namespace Rizzy.Nonce;

/// <summary>
/// Defines the different types of nonces used in Content Security Policies.
/// </summary>
public enum NonceType
{
    /// <summary>
    /// Nonce for inline script tags.
    /// </summary>
    Script,

    /// <summary>
    /// Nonce for inline style tags.
    /// </summary>
    Style
}
