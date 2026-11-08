using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Rizzy.Configuration;
using Rizzy.Htmx;
using Rizzy.Utility;

namespace Rizzy.Tests.Pipeline;

public class ControllerAndServiceTests(RizzyTestHost host) : IClassFixture<RizzyTestHost>
{
    [Fact]
    public async Task RzController_View_with_typed_parameter_builder_renders_full_page()
    {
        var html = await (await host.GetAsync("/ctrl/typed?x=1")).Content.ReadAsStringAsync();

        html.Should().Contain("<p id=\"page\">typed /ctrl/typed?x=1</p>");
        html.Should().Contain("id=\"main-layout\"");
    }

    [Fact]
    public async Task RzController_PartialView_renders_multiple_fragments()
    {
        var html = await (await host.GetAsync("/ctrl/fragments")).Content.ReadAsStringAsync();

        html.Should().Contain("<b id=\"f1\">one</b>").And.Contain("<b id=\"f2\">two</b>");
        html.Should().NotContain("main-layout");
    }

    private static (HtmxSwapService Service, ServiceProvider Provider) CreateSwapService(bool htmx)
    {
        var context = new DefaultHttpContext();
        if (htmx) context.Request.Headers[HtmxRequestHeaderNames.HtmxRequest] = "true";

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions();
        services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor { HttpContext = context });
        services.AddScoped<HtmxSwapService>();
        services.AddScoped<IHtmxSwapService>(sp => sp.GetRequiredService<HtmxSwapService>());

        var provider = services.BuildServiceProvider();
        context.RequestServices = provider;
        return (provider.GetRequiredService<HtmxSwapService>(), provider);
    }

    [Fact]
    public async Task Swap_service_renders_components_markup_and_raw_content()
    {
        var (service, provider) = CreateSwapService(htmx: true);
        await using var _ = provider;

        service.AddSwappableComponent<TestPage>("a", new { Message = "component" });
        service.AddSwappableContent("b", "<i>markup</i>", SwapStyle.innerHTML);
        service.AddSwappableFragment("c", b => b.AddContent(0, "fragment"), SwapStyle.beforeend, selector: ".list");
        service.AddRawContent("<em id=\"raw\">raw</em>");

        service.ContentAvailable.Should().BeTrue();
        var html = await service.RenderToString();

        html.Should().Contain("<hx-partial hx-target=\"#a\" hx-swap=\"outerHTML\"><p id=\"page\">component</p></hx-partial>");
        html.Should().Contain("<hx-partial hx-target=\"#b\" hx-swap=\"innerHTML\"><i>markup</i></hx-partial>");
        html.Should().Contain("<hx-partial hx-target=\".list\" hx-swap=\"beforeend\">fragment</hx-partial>");
        html.Should().Contain("<em id=\"raw\">raw</em>");

        service.Clear();
        service.ContentAvailable.Should().BeFalse();
        (await service.RenderToString()).Should().BeEmpty();
    }

    [Fact]
    public async Task Swap_service_omits_swappable_content_for_non_htmx_requests()
    {
        var (service, provider) = CreateSwapService(htmx: false);
        await using var _ = provider;

        service.AddSwappableContent("b", "<i>markup</i>");
        service.AddRawContent("<em>raw</em>");

        var html = await service.RenderToString();
        html.Should().Contain("<em>raw</em>").And.NotContain("hx-partial");
    }

    [Fact]
    public async Task RenderFragment_helpers_render_markup()
    {
        RenderFragment fragment = b => b.AddMarkupContent(0, "<span>hi</span>");
        await using var provider = new ServiceCollection().AddLogging().BuildServiceProvider();

        fragment.AsMarkupString().Should().Be("<span>hi</span>");
        (await fragment.RenderHtmlAsync(provider)).Should().Be("<span>hi</span>");
        ((RenderFragment?)null).AsMarkupString().Should().BeEmpty();
    }

    [Fact]
    public void RizzyConfig_rejects_invalid_layout_types()
    {
        var config = new RizzyConfig();

        config.Invoking(c => c.DefaultLayout = typeof(string)).Should().Throw<Exception>();
        config.Invoking(c => c.RootComponent = typeof(TestMainLayout)).Should().Throw<Exception>();

        config.DefaultLayout = typeof(TestMainLayout);
        config.RootComponent = typeof(HtmxApp<TestRootLayout>);
        config.DefaultLayout.Should().Be(typeof(TestMainLayout));
    }

    [Fact]
    public void IdGenerator_creates_unique_prefixed_ids()
    {
        var ids = Enumerable.Range(0, 100).Select(_ => IdGenerator.UniqueId("field")).ToList();

        ids.Should().OnlyHaveUniqueItems();
        ids.Should().AllSatisfy(id => id.Should().StartWith("field"));
        IdGenerator.UniqueId("").Should().StartWith("id");
    }
}
