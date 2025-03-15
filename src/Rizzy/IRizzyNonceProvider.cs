using Rizzy.Htmx;

namespace Rizzy;

/// <summary>
/// Service interface for providing CSP nonce values on a scoped basis.
/// </summary>
public interface IRizzyNonceProvider
{
	/// <summary>
	/// Retrieves (or generates) a nonce value for use with content security policies.
	/// </summary>
	/// <returns>A string representing a content security policy compatible nonce</returns>
	string GetNonce();
}