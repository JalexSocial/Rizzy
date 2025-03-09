using Microsoft.AspNetCore.Http;

namespace Rizzy.Nonce;

/// <summary>
/// Provides nonce values for various types (script, style, etc.) for use in Content Security Policies.
/// </summary>
public sealed class RizzyNonceProvider : IRizzyNonceProvider
{
    // Map known nonce types to their respective header names.
    private static readonly Dictionary<NonceType, string> HeaderNames = new ()
        {
            { NonceType.Script, Constants.Headers.ScriptNonce },
            { NonceType.Style, Constants.Headers.StyleNonce }
        };

    // Nonce generator used to create and validate nonce values.
    private readonly RizzyNonceGenerator _generator;
    // HTTP context accessor to retrieve the current HTTP request context.
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="RizzyNonceProvider"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The <see cref="IHttpContextAccessor"/> to access the current HTTP context.</param>
    /// <param name="generator">An instance of <see cref="RizzyNonceGenerator"/> to generate nonce values.</param>
    public RizzyNonceProvider(IHttpContextAccessor httpContextAccessor, RizzyNonceGenerator generator)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _generator = generator;
    }

    /// <summary>
    /// Retrieves the nonce values for the current HTTP request. 
    /// New nonce values are generated and cached if not already present.
    /// </summary>
    /// <returns>An instance of <see cref="RizzyNonceValues"/> containing nonce mappings for various nonce types.</returns>
    public RizzyNonceValues GetNonceValues()
    {
        var context = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("No HttpContext available.");

        // If nonce values have already been generated for this request, return them.
        if (context.Items[Constants.NonceKey] is RizzyNonceValues cachedNonceValues)
        {
            return cachedNonceValues;
        }

        // Create a new nonce values container.
        var nonceValues = new RizzyNonceValues();

        // For each nonce type that has a header mapping, attempt to retrieve its value from the request.
        foreach (var kvp in HeaderNames)
        {
            var nonceType = kvp.Key;
            var headerName = kvp.Value;

            context.Request.Headers.TryGetValue(headerName, out var headerValues);
            var nonce = headerValues.FirstOrDefault();

            // Validate the provided nonce; if invalid or missing, generate a new one.
            if (string.IsNullOrEmpty(nonce) || !_generator.ValidateNonce(nonce))
            {
                nonce = _generator.CreateNonce();
            }

            nonceValues.SetNonce(nonceType, nonce);
        }

        // Cache the nonce values for the current request.
        context.Items[Constants.NonceKey] = nonceValues;
        return nonceValues;
    }

    /// <summary>
    /// Retrieves (or generates if necessary) the nonce value for the specified nonce type.
    /// </summary>
    /// <param name="nonceType">The type of nonce required (for example, Script or Style).</param>
    /// <returns>The nonce string for the specified type.</returns>
    public string GetNonceFor(NonceType nonceType)
    {
        var nonceValues = GetNonceValues();

        // If a nonce already exists for the given type, return it.
        var currentNonce = nonceValues.GetNonce(nonceType);
        if (!string.IsNullOrEmpty(currentNonce))
        {
            return currentNonce;
        }

        // Otherwise, generate a new nonce, store it, and return the new value.
        var newNonce = _generator.CreateNonce();
        nonceValues.SetNonce(nonceType, newNonce);
        return newNonce;
    }
}


