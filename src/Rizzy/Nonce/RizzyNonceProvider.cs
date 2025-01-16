using Microsoft.AspNetCore.Http;

namespace Rizzy.Nonce;

/// <summary>
/// Provides nonce values for inline scripts and styles to enhance security by preventing
/// the execution of unauthorized scripts and styles in web applications.
/// </summary>
public sealed class RizzyNonceProvider : IRizzyNonceProvider
{
    /// <summary>
    /// The key used to store nonce values in the <see cref="HttpContext.Items"/> collection.
    /// </summary>
    private static readonly string NonceKey = "RizzyNonceValues";

    /// <summary>
    /// Defines the custom header name for the inline script nonce.
    /// </summary>
    private static readonly string ScriptNonceHeader = "Rizzy-Script-Nonce";

    /// <summary>
    /// Defines the custom header name for the inline style nonce.
    /// </summary>
    private static readonly string StyleNonceHeader = "Rizzy-Style-Nonce";

    /// <summary>
    /// Generates secure nonce values
    /// </summary>
    private readonly RizzyNonceGenerator _generator;

    /// <summary>
    /// The HTTP context accessor used to retrieve the current <see cref="HttpContext"/>.
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="RizzyNonceProvider"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">
    /// The <see cref="IHttpContextAccessor"/> used to access the current <see cref="HttpContext"/>.
    /// </param>
    /// <param name="generator"></param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="httpContextAccessor"/> is <c>null</c>.
    /// </exception>
    public RizzyNonceProvider(IHttpContextAccessor httpContextAccessor, RizzyNonceGenerator generator)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _generator = generator;
    }

    /// <summary>
    /// Retrieves the nonce values for inline scripts and styles. If the nonce values
    /// have already been generated for the current HTTP request or provided via headers,
    /// they are returned from the cache; otherwise, new nonce values are generated, cached, and returned.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="RizzyNonceValues"/> containing the generated or reused nonce values.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if there is no current <see cref="HttpContext"/>.
    /// </exception>
    public RizzyNonceValues GetNonceValues()
    {
        var context = _httpContextAccessor.HttpContext
                      ?? throw new InvalidOperationException("No HttpContext available.");

        // If it's cached already return the existing nonce value object
        if (context.Items[NonceKey] is RizzyNonceValues cachedNonceValues)
        {
            return cachedNonceValues;
        }

        if (context.Request.IsHtmx())
        {
            // Attempt to retrieve nonce values from headers
            context.Request.Headers.TryGetValue(ScriptNonceHeader, out var scriptNonceValues);
            context.Request.Headers.TryGetValue(StyleNonceHeader, out var styleNonceValues);

            var scriptNonce = scriptNonceValues.FirstOrDefault();
            var styleNonce = styleNonceValues.FirstOrDefault();

            // If nonce values aren't present or we can't validate them then
            // generate new nonce values
            if (string.IsNullOrEmpty(scriptNonce) || !_generator.ValidateNonce(scriptNonce))
            {
                scriptNonce = _generator.CreateNonce();
            }

            if (string.IsNullOrEmpty(styleNonce) || !_generator.ValidateNonce(styleNonce))
            {
                styleNonce = _generator.CreateNonce();
            }
        }

        // Build updated nonce values
        var nonceValues = new RizzyNonceValues
        {
            InlineScriptNonce = scriptNonce,
            InlineStyleNonce = styleNonce
        };
        context.Items[NonceKey] = nonceValues;

        return nonceValues;
    }

    /// <summary>
    /// Gets the nonce value to be used for inline scripts in the current HTTP request.
    /// </summary>
    public string InlineScriptNonce => GetNonceValues().InlineScriptNonce;

    /// <summary>
    /// Gets the nonce value to be used for inline styles in the current HTTP request.
    /// </summary>
    public string InlineStyleNonce => GetNonceValues().InlineStyleNonce;
}

