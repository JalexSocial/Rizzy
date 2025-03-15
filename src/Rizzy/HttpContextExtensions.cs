using System.Security.Cryptography;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Rizzy;

public static class HttpContextExtensions
{
    /// <summary>
    /// Retrieves the nonce values for the current HTTP request. New nonce values are generated and cached if not already present.
    /// The recommended way to generate nonce values is to utilize an IRizzyNonceProvider service implementation directly that can
    /// be injected into a Controller, Component, View, etc.
    /// </summary>
    /// <returns>An instance of <see cref="string"/> containing nonce value.</returns>
    public static string GetNonce(this HttpContext context)
    {
	    var provider = context.RequestServices.GetRequiredService<IRizzyNonceProvider>();

	    return provider.GetNonce();
    }

}