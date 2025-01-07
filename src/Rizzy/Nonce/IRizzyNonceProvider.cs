namespace Rizzy.Nonce;

/// <summary>
/// Service interface for providing CSP nonce values on a scoped basis.
/// </summary>
public interface IRizzyNonceProvider
{
    /// <summary>
    /// Retrieves the nonce values for inline scripts and styles. If the nonce values
    /// have already been generated for the current HTTP request, they are returned
    /// from the cache; otherwise, new nonce values are generated, cached, and returned.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="RizzyNonceValues"/> containing the generated nonce values.
    /// </returns>
    public RizzyNonceValues GetNonceValues();

    /// <summary>
    /// The nonce to add to any inlined scripts
    /// </summary>
    public string InlineScriptNonce { get; }

    /// <summary>
    /// The nonce to add to any inlined styles
    /// </summary>
    public string InlineStyleNonce { get; }
}
