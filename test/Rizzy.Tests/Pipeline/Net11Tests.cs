using System.Net;

namespace Rizzy.Tests.Pipeline;

/// <summary>
/// Behavior that differs between the net10.0 and net11.0 builds: CSRF protection for Rizzy's form
/// mapping, async validation, and RzPageTitle inside a CacheView.
/// </summary>
public class Net11Tests(RizzyTestHost host) : IClassFixture<RizzyTestHost>
{
    private Task<HttpResponseMessage> PostForm(string url, string secFetchSite)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent([new("_handler", "greet"), new("Name", "Ada")]),
        };
        request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", secFetchSite);
        return host.Client.SendAsync(request);
    }

    [Fact]
    public async Task Same_origin_form_post_is_mapped_without_a_token()
    {
        var response = await PostForm("/minimal-post-protected", "same-origin");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).Should().Contain("posted:Ada");
    }

    [Fact]
    public async Task Cross_site_form_post_to_an_endpoint_that_opted_out_is_mapped()
        => (await PostForm("/minimal-post", "cross-site")).StatusCode.Should().Be(HttpStatusCode.OK);

#if NET11_0_OR_GREATER
    [Fact]
    public async Task Cross_site_form_post_is_rejected_before_mapping()
        => (await PostForm("/minimal-post-protected", "cross-site")).StatusCode.Should().Be(HttpStatusCode.BadRequest);
#else
    [Fact]
    public async Task Cross_site_form_post_is_mapped_on_net10_which_has_no_automatic_csrf_check()
        => (await PostForm("/minimal-post-protected", "cross-site")).StatusCode.Should().Be(HttpStatusCode.OK);
#endif

    [Fact]
    public async Task RzPageTitle_Text_sets_the_title()
        => (await (await host.GetAsync("/title-text")).Content.ReadAsStringAsync()).Should().Contain("<title>Text title</title>");

    [Fact]
    public async Task RzPageTitle_Text_renders_an_inline_title_for_partial_requests()
        => (await (await host.GetAsync("/title-text", RizzyTestHost.PartialHtmxHeaders)).Content.ReadAsStringAsync())
            .Should().Contain("<title>Text title</title>");

#if NET11_0_OR_GREATER
    [Fact]
    public async Task RzPageTitle_inside_a_CacheView_keeps_the_title_on_a_cache_hit()
    {
        var first = await (await host.GetAsync("/cached-title")).Content.ReadAsStringAsync();
        var renders = TestRenderCounter.Renders;
        var second = await (await host.GetAsync("/cached-title")).Content.ReadAsStringAsync();

        TestRenderCounter.Renders.Should().Be(renders, "the second response comes from the cache");
        first.Should().Contain("<title>Cached title</title>");
        second.Should().Contain("<title>Cached title</title>");
    }

    [Fact]
    public async Task RzInitialValidator_runs_async_validators()
    {
        var response = await host.GetAsync("/async-validate");
        var html = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, html);
        html.Should().Contain("That username is taken.");
    }
#endif
}
