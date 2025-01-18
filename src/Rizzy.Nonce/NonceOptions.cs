using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rizzy.Nonce;

/// <summary>
/// Options for configuring nonce generation
/// </summary>
public class NonceOptions
{
	/// <summary>
	/// Gets or sets the HMAC key as a base‑64 encoded string.
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public string HMACKey { get; set; } = string.Empty;

	/// <summary>
	/// Gets the decoded Nonce HMAC Key as a byte array.
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public byte[] HMACKeyBytes => Convert.FromBase64String(HMACKey);
}
