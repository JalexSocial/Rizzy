using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rizzy.Htmx;

namespace Rizzy.Tests.Pipeline;

/// <summary>
/// htmx 4 error, validation and authentication responses (RIZZY-019).
/// </summary>
public sealed class ErrorHandlingTests : IAsyncLifetime
{
    private WebApplication? _app;
    private HttpClient _client = default!;

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Testing" });
        builder.WebHost.UseTestServer();
        builder.Logging.ClearProviders();
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(o => o.Events.UseHtmxRedirects());
        builder.Services.AddAuthorization();

        var app = builder.Build();
        app.UseHtmxErrorHandling(o => o.ErrorTarget = app.Configuration["ErrorTarget"]);
        app.UseExceptionHandler(error => error.Run(ctx =>
        {
            ctx.Response.StatusCode = 500;
            return ctx.Response.WriteAsync("<html><body><h1>Something went wrong</h1></body></html>");
        }));
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGet("/throw", (Func<string>)(() => throw new InvalidOperationException("boom")));
        app.MapGet("/handled", (HttpContext ctx) =>
        {
            ctx.Response.Htmx().Retarget("#custom");
            return Results.Content("<p>custom</p>", "text/html", statusCode: 500);
        });
        app.MapPost("/save", (HttpContext ctx) =>
        {
            ctx.Response.Htmx().ValidationFailed("#form", SwapStyle.outerHTML);
            return Results.Content("<form id=\"form\"><span>Name is required</span></form>", "text/html");
        });
        app.MapGet("/secure", () => "secret").RequireAuthorization();

        await app.StartAsync();
        _app = app;
        _client = app.GetTestClient();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        if (_app is not null) await _app.DisposeAsync();
    }

    private Task<HttpResponseMessage> Send(HttpMethod method, string url, params (string, string)[] headers)
    {
        var request = new HttpRequestMessage(method, url);
        foreach (var (name, value) in headers) request.Headers.TryAddWithoutValidation(name, value);
        return _client.SendAsync(request);
    }

    private static string? Header(HttpResponseMessage response, string name)
        => response.Headers.TryGetValues(name, out var values) ? string.Join(",", values) : null;

    [Fact]
    public async Task Exception_on_partial_htmx_request_is_not_swapped_and_raises_an_event()
    {
        var response = await Send(HttpMethod.Get, "/throw", RizzyTestHost.PartialHtmxHeaders);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        Header(response, "HX-Reswap").Should().Be("none");
        Header(response, "HX-Retarget").Should().BeNull();

        using var trigger = JsonDocument.Parse(Header(response, "HX-Trigger")!);
        trigger.RootElement.GetProperty("rz:server-error").GetProperty("status").GetInt32().Should().Be(500);
    }

    [Fact]
    public async Task Exception_on_full_page_htmx_request_is_left_alone()
    {
        var response = await Send(HttpMethod.Get, "/throw", RizzyTestHost.FullPageHtmxHeaders);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        Header(response, "HX-Reswap").Should().BeNull();
        (await response.Content.ReadAsStringAsync()).Should().Contain("Something went wrong");
    }

    [Fact]
    public async Task Exception_on_normal_request_is_left_alone()
    {
        var response = await Send(HttpMethod.Get, "/throw");

        Header(response, "HX-Reswap").Should().BeNull();
        Header(response, "HX-Trigger").Should().BeNull();
    }

    [Fact]
    public async Task Explicit_retarget_is_respected()
    {
        var response = await Send(HttpMethod.Get, "/handled", RizzyTestHost.PartialHtmxHeaders);

        Header(response, "HX-Retarget").Should().Be("#custom");
        Header(response, "HX-Reswap").Should().BeNull();
    }

    [Fact]
    public async Task ValidationFailed_returns_422_with_retarget_and_reswap()
    {
        var response = await Send(HttpMethod.Post, "/save", RizzyTestHost.PartialHtmxHeaders);

        response.StatusCode.Should().Be((HttpStatusCode)422);
        Header(response, "HX-Retarget").Should().Be("#form");
        Header(response, "HX-Reswap").Should().Be("outerHTML");
        (await response.Content.ReadAsStringAsync()).Should().Contain("Name is required");
    }

    [Fact]
    public async Task Login_challenge_on_htmx_request_uses_HX_Redirect()
    {
        var response = await Send(HttpMethod.Get, "/secure", RizzyTestHost.PartialHtmxHeaders);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        Header(response, "HX-Redirect").Should().StartWith("http://localhost/Account/Login?ReturnUrl=");
        response.Headers.Location.Should().BeNull();
    }

    [Fact]
    public async Task Login_challenge_on_normal_request_still_redirects()
    {
        var response = await Send(HttpMethod.Get, "/secure");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        Header(response, "HX-Redirect").Should().BeNull();
    }
}

public class HtmxStatusTests
{
    [Theory]
    [InlineData(422, "hx-status:422")]
    [InlineData(500, "hx-status:500")]
    public void Attribute_for_code(int code, string expected) => HtmxStatus.Attribute(code).Should().Be(expected);

    [Theory]
    [InlineData("4xx", "hx-status:4xx")]
    [InlineData("40X", "hx-status:40x")]
    [InlineData("404", "hx-status:404")]
    public void Attribute_for_pattern(string pattern, string expected) => HtmxStatus.Attribute(pattern).Should().Be(expected);

    [Theory]
    [InlineData("4x2")]
    [InlineData("600")]
    [InlineData("x00")]
    [InlineData("hx-status:422")]
    public void Invalid_patterns_throw(string pattern)
        => FluentActions.Invoking(() => HtmxStatus.Attribute(pattern)).Should().Throw<ArgumentException>();

    [Fact]
    public void Out_of_range_code_throws()
        => FluentActions.Invoking(() => HtmxStatus.Attribute(99)).Should().Throw<ArgumentOutOfRangeException>();
}
