using System.Security.Cryptography;

namespace Rizzy.Nonce;

/// <summary>
/// Utility class to generate tamper-resistant nonce values that can be passed back to the
/// server for reuse
/// </summary> 
public sealed class NonceUtil
{
    /// <summary>
    /// Creates a tamper-resistant nonce consisting of:
    /// - 16 random bytes (128 bits) as the nonce payload
    /// - 16-byte HMAC signature
    /// 
    /// The result is ~44 characters in Base64 when encoded.
    /// </summary>
    /// <param name="hmacKey">A secret key used for HMAC signing.</param>
    /// <returns>Base64-encoded token combining the random data and its signature.</returns>
    public static string CreateNonce(byte[] hmacKey)
    {
        if (hmacKey == null || hmacKey.Length == 0)
        {
            throw new ArgumentException("HMAC key must be provided and cannot be empty.", nameof(hmacKey));
        }

        // 1) Generate 16 random bytes (128 bits)
        byte[] randomBytes = new byte[16];
        RandomNumberGenerator.Fill(randomBytes);

        // 2) Compute HMAC over those 16 bytes
        byte[] fullHmac;
        using (var hmac = new HMACSHA256(hmacKey))
        {
            fullHmac = hmac.ComputeHash(randomBytes); // 32 bytes
        }

        // 3) Take the first 16 bytes of the HMAC as our signature
        byte[] signature = new byte[16];
        Array.Copy(fullHmac, 0, signature, 0, 16);

        // 4) Combine random bytes (16) + signature (16) => 32 bytes
        byte[] nonceWithSignature = new byte[32];
        Array.Copy(randomBytes, 0, nonceWithSignature, 0, 16);
        Array.Copy(signature, 0, nonceWithSignature, 16, 16);

        // 5) Convert to Base64 => ~44 chars
        return Convert.ToBase64String(nonceWithSignature);
    }

    /// <summary>
    /// Validates a nonce that was produced by <see cref="CreateNonce"/>.
    /// Ensures it has not been tampered with by recomputing the HMAC signature.
    /// </summary>
    /// <param name="nonceToken">The Base64-encoded token (~44 chars).</param>
    /// <param name="hmacKey">Same secret key used in <see cref="CreateNonce"/>.</param>
    /// <returns>True if valid; false otherwise.</returns>
    public static bool ValidateNonce(string nonceToken, byte[] hmacKey)
    {
        if (string.IsNullOrWhiteSpace(nonceToken))
            return false;
        if (hmacKey.Length == 0)
            return false;

        // 1) Base64 decode the token
        byte[] nonceBytes;
        try
        {
            nonceBytes = Convert.FromBase64String(nonceToken);
        }
        catch
        {
            // Invalid Base64
            return false;
        }

        // 2) Expect exactly 32 bytes (16 random + 16 signature)
        if (nonceBytes.Length != 32)
        {
            return false;
        }

        // 3) Split into randomBytes and signature
        byte[] randomBytes = new byte[16];
        byte[] signature = new byte[16];

        Array.Copy(nonceBytes, 0, randomBytes, 0, 16);
        Array.Copy(nonceBytes, 16, signature, 0, 16);

        // 4) Recompute the HMAC over the randomBytes
        byte[] fullHmac;
        using (var hmac = new HMACSHA256(hmacKey))
        {
            fullHmac = hmac.ComputeHash(randomBytes); // 32 bytes
        }

        // 5) Extract the first 16 bytes as the expected signature
        byte[] expectedSignature = new byte[16];
        Array.Copy(fullHmac, 0, expectedSignature, 0, 16);

        // 6) Use a time-constant comparison to prevent timing attacks
        return CryptographicOperations.FixedTimeEquals(expectedSignature, signature);
    }
}
