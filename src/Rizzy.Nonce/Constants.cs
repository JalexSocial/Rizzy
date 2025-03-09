namespace Rizzy.Nonce;

/// <summary>
/// Defined constants for Rizzy.Nonce
/// </summary>
internal static class Constants
{
    /// <summary>
    /// Nonce headers for use with HTMX
    /// </summary>
	internal static class Headers
	{
		public const string ScriptNonce = "Rizzy-Script-Nonce";
		public const string StyleNonce = "Rizzy-Style-Nonce";
	}

    /// <summary>
    /// Key to store nonce values in HttpContext.Items.
    /// </summary>
	public const string NonceKey = "RizzyNonceValues";
}

