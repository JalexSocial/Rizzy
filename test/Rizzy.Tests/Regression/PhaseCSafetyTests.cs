using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Rizzy.Htmx;

namespace Rizzy.Tests.Regression;

/// <summary>Safe-by-name markup APIs and selector escaping (RIZZY-029, RIZZY-031, RM-C5).</summary>
public class PhaseCSafetyTests
{
    private static (HtmxSwapService Service, ServiceProvider Provider) CreateSwapService()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[HtmxRequestHeaderNames.HtmxRequest] = "true";

        var services = new ServiceCollection().AddLogging().AddOptions();
        services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor { HttpContext = context });
        services.AddScoped<HtmxSwapService>();
        services.AddScoped<IHtmxSwapService>(sp => sp.GetRequiredService<HtmxSwapService>());

        var provider = services.BuildServiceProvider();
        context.RequestServices = provider;
        return (provider.GetRequiredService<HtmxSwapService>(), provider);
    }

    [Fact]
    public async Task Swappable_text_is_html_encoded()
    {
        var (service, provider) = CreateSwapService();
        await using var _ = provider;

        service.AddSwappableText("msg", "<script>alert(1)</script>");

        var html = await service.RenderToString();
        html.Should().Contain("&lt;script&gt;alert(1)&lt;/script&gt;").And.NotContain("<script>");
    }

    [Fact]
    public async Task Target_ids_are_css_escaped()
    {
        var (service, provider) = CreateSwapService();
        await using var _ = provider;

        service.AddSwappableText("item:1", "x");
        service.AddSwappableText("2nd", "y");

        var html = await service.RenderToString();
        html.Should().Contain("hx-target=\"#item\\:1\"").And.Contain("hx-target=\"#\\32 nd\"");
    }

    [Fact]
    public void Swap_service_resolves_HttpContext_lazily()
    {
        var accessor = new HttpContextAccessor();
        var service = new HtmxSwapService(accessor, new ServiceCollection().BuildServiceProvider());

        var context = new DefaultHttpContext();
        context.Request.Headers[HtmxRequestHeaderNames.HtmxRequest] = "true";
        accessor.HttpContext = context;

        service.AddSwappableText("t", "late");
        var fragmentOwner = service.RenderToFragment();
        fragmentOwner.Should().NotBeNull();
    }

    [Theory]
    [InlineData("plain", "plain")]
    [InlineData("a.b", "a\\.b")]
    [InlineData("1x", "\\31 x")]
    [InlineData("-1", "-\\31 ")]
    [InlineData("-", "\\-")]
    [InlineData("a b", "a\\ b")]
    [InlineData("ünï", "ünï")]
    public void Css_escape_matches_the_CSSOM_algorithm(string input, string expected)
        => CssSelector.Escape(input).Should().Be(expected);

    [Theory]
    [InlineData("/", true)]
    [InlineData("/a/b?c=d", true)]
    [InlineData("~/a", true)]
    [InlineData("//evil.example", false)]
    [InlineData("/\\evil.example", false)]
    [InlineData("https://evil.example", false)]
    [InlineData("~//evil.example", false)]
    [InlineData("", false)]
    public void IsLocalUrl(string url, bool expected) => HtmxResponse.IsLocalUrl(url).Should().Be(expected);

    [Fact]
    public void LocalRedirect_rejects_external_urls_and_resolves_app_relative_ones()
    {
        var context = new DefaultHttpContext { RequestServices = new ServiceCollection().BuildServiceProvider() };
        context.Request.Headers[HtmxRequestHeaderNames.HtmxRequest] = "true";
        context.Request.PathBase = "/app";
        var response = context.Response.Htmx();

        response.Invoking(r => r.LocalRedirect("https://evil.example")).Should().Throw<InvalidOperationException>();

        response.LocalRedirect("~/home");
        context.Response.Headers[HtmxResponseHeaderNames.Redirect].ToString().Should().Be("/app/home");
    }

    [Fact]
    public void Raw_markup_apis_require_an_explicit_MarkupString()
    {
        typeof(IHtmxSwapService).GetMethod(nameof(IHtmxSwapService.AddRawContent))!
            .GetParameters()[0].ParameterType.Should().Be(typeof(MarkupString));
        typeof(IHtmxSwapService).GetMethod(nameof(IHtmxSwapService.AddSwappableContent))!
            .GetParameters()[1].ParameterType.Should().Be(typeof(MarkupString));
    }

    // ---- RIZZY-030: consistent HtmxResponse assertion policy ------------------------------------

    private static HttpContext Context(bool htmx)
    {
        var context = new DefaultHttpContext { RequestServices = new ServiceCollection().BuildServiceProvider() };
        if (htmx) context.Request.Headers[HtmxRequestHeaderNames.HtmxRequest] = "true";
        return context;
    }

    [Fact]
    public void Every_reswap_overload_requires_an_htmx_request()
    {
        var response = Context(htmx: false).Response.Htmx();

        response.Invoking(r => r.Reswap("innerHTML")).Should().Throw<InvalidOperationException>();
        response.Invoking(r => r.Reswap(SwapStyle.outerHTML)).Should().Throw<InvalidOperationException>();
        response.Invoking(r => r.Reswap(new SwapStyleBuilder(SwapStyle.innerHTML))).Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void StatusCode_works_for_any_request()
    {
        var context = Context(htmx: false);
        context.Response.Htmx().StatusCode(System.Net.HttpStatusCode.Accepted);
        context.Response.StatusCode.Should().Be(202);
    }

    [Fact]
    public void Reswap_default_style_keeps_the_requested_style()
    {
        var context = Context(htmx: true);
        var response = context.Response.Htmx();

        response.Reswap(SwapStyle.Default);
        context.Response.Headers.ContainsKey(HtmxResponseHeaderNames.Reswap).Should().BeFalse();

        response.Reswap(SwapStyle.Default, "scroll:top");
        context.Response.Headers[HtmxResponseHeaderNames.Reswap].ToString().Should().Be("scroll:top");
    }
}
