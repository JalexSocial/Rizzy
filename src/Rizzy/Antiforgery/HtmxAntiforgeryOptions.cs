using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Options;

namespace Rizzy.Antiforgery;

/// <summary>
/// Represents the options for Rizzy's antiforgery support.
/// </summary>
public class HtmxAntiforgeryOptions
{
	public bool IncludeAntiForgery { get; set; } = true;

	/// <summary>
	/// Gets the name of the form field used for antiforgery token.
	/// </summary>
	public string FormFieldName { get; set; } = default!;

	/// <summary>
	/// Gets the name of the header used for antiforgery token.
	/// </summary>
	public string? HeaderName { get; set; }

	/// <summary>
	/// Gets the name of the cookie used for antiforgery token.
	/// </summary>
	public string CookieName { get; set; } = "HX-XSRF-TOKEN";
}
