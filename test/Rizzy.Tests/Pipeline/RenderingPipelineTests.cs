using System.Net;
using System.Text.RegularExpressions;

namespace Rizzy.Tests.Pipeline;

/// <summary>
/// End-to-end tests through a real ASP.NET Core pipeline (RIZZY-025).
/// Tests for known, not-yet-fixed defects are skipped and name the audit finding they cover;
/// remove the Skip when the finding is fixed.
/// </summary>
public class RenderingPipelineTests(RizzyTestHost host) : IClassFixture<RizzyTestHost>
{
    private const string KnownDefect = "Known defect, see docs/audits/2026-11-08-rizzy-initial-audit";

    [Fact]
    public async Task View_without_htmx_renders_full_layout_and_htmx_config()
    {
        var response = await host.GetAsync("/page");
        var html = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        html.Should().Contain("data-layout=\"root\"");
        html.Should().Contain("id=\"main-layout\"");
        html.Should().Contain("<p id=\"page\">hello page</p>");
        html.Should().Contain("<meta name=\"htmx-config\"");
    }

    [Fact]
    public async Task Htmx_config_meta_contains_antiforgery_token_and_document_nonce()
    {
        var html = await (await host.GetAsync("/page")).Content.ReadAsStringAsync();

        var meta = Regex.Match(html, "<meta name=\"htmx-config\" content='([^']*)'>");
        meta.Success.Should().BeTrue();
        meta.Groups[1].Value.Should().Contain("\"antiforgery\":{")
            .And.Contain("\"headerName\":\"RequestVerificationToken\"")
            .And.Contain("\"requestToken\":\"")
            .And.Contain("\"documentNonce\":\"");
    }

    [Fact]
    public async Task View_for_partial_htmx_request_renders_fragment_without_layout()
    {
        var response = await host.GetAsync("/page", RizzyTestHost.PartialHtmxHeaders);
        var html = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        html.Should().Contain("<p id=\"page\">hello page</p>");
        html.Should().NotContain("id=\"main-layout\"");
        html.Should().NotContain("<meta name=\"htmx-config\"");
    }

    [Fact]
    public async Task Htmx_responses_vary_on_HX_Request()
    {
        var response = await host.GetAsync("/page", RizzyTestHost.PartialHtmxHeaders);

        response.Headers.Vary.Should().Contain("HX-Request");
    }

    [Fact(Skip = KnownDefect + " RIZZY-018: Vary: HX-Request is only sent on htmx responses")]
    public async Task Full_page_responses_also_vary_on_HX_Request()
    {
        var response = await host.GetAsync("/page");

        response.Headers.Vary.Should().Contain("HX-Request");
    }

    [Fact(Skip = KnownDefect + " RIZZY-006: history restore must render the full layout")]
    public async Task History_restore_request_renders_full_layout()
    {
        var html = await (await host.GetAsync("/page", RizzyTestHost.HistoryRestoreHeaders)).Content.ReadAsStringAsync();

        html.Should().Contain("id=\"main-layout\"");
    }

    [Fact(Skip = KnownDefect + " RIZZY-006: HX-Request-Type: full must render the full layout")]
    public async Task Full_page_htmx_request_renders_full_layout()
    {
        var html = await (await host.GetAsync("/page", RizzyTestHost.FullPageHtmxHeaders)).Content.ReadAsStringAsync();

        html.Should().Contain("id=\"main-layout\"");
    }

    [Fact]
    public async Task PartialView_never_renders_a_layout()
    {
        var html = await (await host.GetAsync("/partial")).Content.ReadAsStringAsync();

        html.Should().Contain("<p id=\"page\">hello partial</p>");
        html.Should().NotContain("data-layout=\"root\"");
        html.Should().NotContain("id=\"main-layout\"");
    }

    [Fact]
    public async Task EmptyBody_returns_204()
    {
        var response = await host.GetAsync("/empty", RizzyTestHost.PartialHtmxHeaders);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await response.Content.ReadAsStringAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task Redirect_returns_204_with_HX_Redirect_header()
    {
        var response = await host.GetAsync("/redirect", RizzyTestHost.PartialHtmxHeaders);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response.Headers.GetValues("HX-Redirect").Should().ContainSingle().Which.Should().Be("/elsewhere");
    }

    [Fact]
    public async Task Htmx_requests_get_an_HX_Nonce_header()
    {
        var response = await host.GetAsync("/partial", RizzyTestHost.PartialHtmxHeaders);

        response.Headers.GetValues("HX-Nonce").Should().ContainSingle().Which.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Swap_service_emits_hx_partial_for_htmx_requests_only()
    {
        var htmx = await (await host.GetAsync("/swap", RizzyTestHost.PartialHtmxHeaders)).Content.ReadAsStringAsync();
        var plain = await (await host.GetAsync("/swap")).Content.ReadAsStringAsync();

        htmx.Should().Contain("<p id=\"swap-page\">main content</p>");
        htmx.Should().Contain("<hx-partial hx-target=\"#oob-target\" hx-swap=\"outerHTML\">oob content</hx-partial>");
        plain.Should().Contain("main content").And.NotContain("hx-partial");
    }

    [Fact]
    public async Task HtmxResponseAttribute_is_ignored_for_non_htmx_requests()
    {
        // RIZZY-009: this used to throw InvalidOperationException (HTTP 500).
        var response = await host.GetAsync("/attr");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Contains("HX-Push-Url").Should().BeFalse();
        (await response.Content.ReadAsStringAsync()).Should().Contain("attribute");
    }

    [Fact]
    public async Task HtmxResponseAttribute_sets_headers_for_htmx_requests()
    {
        var response = await host.GetAsync("/attr", RizzyTestHost.PartialHtmxHeaders);

        response.Headers.GetValues("HX-Push-Url").Should().ContainSingle().Which.Should().Be("/pushed");
        response.Headers.GetValues("HX-Refresh").Should().ContainSingle().Which.Should().Be("true");
        response.Headers.GetValues("HX-Replace-Url").Should().ContainSingle().Which.Should().Be("false");
        response.Headers.GetValues("HX-Reswap").Should().ContainSingle().Which.Should().Be("outerHTML");
    }
}
