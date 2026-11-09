using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Rizzy.Htmx;

namespace Rizzy;

/// <summary>
/// The result returned by Rizzy's <c>View</c> and <c>PartialView</c> methods.
/// </summary>
/// <remarks>
/// At execution time it (1) returns 204 No Content if <see cref="HtmxResponse.EmptyBody"/>,
/// <see cref="HtmxResponse.Redirect(string)"/> or <see cref="HtmxResponse.Refresh"/> asked for an
/// empty body, (2) supplies the posted form to Blazor form mapping so
/// <c>[SupplyParameterFromForm]</c> works for controllers and minimal APIs alike, and then
/// (3) renders the component through <see cref="RazorComponentResult"/>.
/// </remarks>
public sealed class RizzyViewResult : IResult
{
    private readonly RazorComponentResult _inner;

    internal RizzyViewResult(RazorComponentResult inner)
    {
        _inner = inner;
    }

    /// <summary>The component parameters passed to the Rizzy view host.</summary>
    public IReadOnlyDictionary<string, object?> Parameters => _inner.Parameters;

    /// <inheritdoc />
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        if (httpContext.Response.Htmx().EmptyResponseBodyRequested)
        {
            httpContext.Response.StatusCode = StatusCodes.Status204NoContent;
            return;
        }

        await BlazorFormData.InitializeAsync(httpContext).ConfigureAwait(false);
        await _inner.ExecuteAsync(httpContext).ConfigureAwait(false);
    }
}
