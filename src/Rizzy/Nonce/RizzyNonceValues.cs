namespace Rizzy.Nonce;

/// <summary>
/// Holds the script and style nonce values to be used in the application.
/// </summary>
public sealed class RizzyNonceValues
{
    /// <summary>
    /// Default empty nonce
    /// </summary>
	public static RizzyNonceValues DefaultValue => new RizzyNonceValues
    { InlineScriptNonce = string.Empty, InlineStyleNonce = string.Empty };

    /// <summary>
    /// The nonce to add to any inlined scripts
    /// </summary>
    public required string InlineScriptNonce { get; set; }

    /// <summary>
    /// The nonce to add to any inlined styles
    /// </summary>
    public required string InlineStyleNonce { get; set; }
}
