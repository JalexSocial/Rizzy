using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace Rizzy.Nonce;

/// <summary>
/// Responsible for generating tamper-resistant nonce values <see cref="NonceUtil"/>
/// </summary>
public sealed class RizzyNonceGenerator
{
    private readonly byte[] _hmacKey;

    /// <summary>
    /// Construct a nonce with an optional pre-existing base-64 encoded HMAC key
    /// </summary>
    /// <remarks>HMAC key must be base-64 encoded and at least 256 bits (32 bytes)</remarks>
    /// <param name="options">Configuration options for RizzyUI</param>
    /// <exception cref="ArgumentException"></exception>
	public RizzyNonceGenerator(IOptions<NonceOptions> options)
    {
        if (string.IsNullOrEmpty(options.Value.HMACKey))
            _hmacKey = GenerateSecureHmacKey();
        else
        {
            try
            {
                _hmacKey = options.Value.HMACKeyBytes;

                if (_hmacKey.Length < 32)
                {
                    throw new ArgumentException("Provided HMAC key when decoded must be at least 256 bits (32 bytes).");
                }
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("NonceHMACKey must be a valid base-64 encoded string.", ex);
            }
        }
    }

    /// <summary>
    /// Creates a tamper-resistant nonce.
    /// </summary>
    /// <returns>Base64-encoded nonce token.</returns>
    public string CreateNonce() => NonceUtil.CreateNonce(_hmacKey);

    /// <summary>
    /// Validates a tamper-resistant nonce.
    /// </summary>
    /// <param name="nonceToken">The Base64-encoded nonce token to validate.</param>
    /// <returns>True if valid; false otherwise.</returns>
    public bool ValidateNonce(string nonceToken) => NonceUtil.ValidateNonce(nonceToken, _hmacKey);

    /// <summary>
    /// Generates a secure HMAC key using a cryptographically secure random number generator.
    /// </summary>
    /// <param name="keySizeInBytes">Size of the key in bytes (e.g., 32 for 256-bit).</param>
    /// <returns>Base64-encoded HMAC key.</returns>
    private byte[] GenerateSecureHmacKey(int keySizeInBytes = 32)
    {
        byte[] key = new byte[keySizeInBytes];
        RandomNumberGenerator.Fill(key);
        return key;
    }
}
