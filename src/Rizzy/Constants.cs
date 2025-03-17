namespace Rizzy;

internal class Constants
{
    /// <summary>
    /// Header name for the nonce value in the response if the request is an HTMX request.
    /// </summary>
    public const string NonceResponseHeader = "HX-Nonce";

    internal static class HttpContextKeys
    {
        /// <summary>
        /// Key to store nonce values in HttpContext.Items.
        /// </summary>
        public const string NonceKey = "RizzyNonce";
    }
}

