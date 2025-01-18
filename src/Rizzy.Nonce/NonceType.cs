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
    Style,

    /// <summary>
    /// Nonce for font requests.
    /// </summary>
    Font,

    /// <summary>
    /// Nonce for image elements.
    /// </summary>
    Image,

    /// <summary>
    /// Nonce for connect requests (e.g., WebSocket connections).
    /// </summary>
    Connect
}
