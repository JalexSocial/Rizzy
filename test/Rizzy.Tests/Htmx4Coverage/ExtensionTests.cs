using System.Text.Json;
using Bunit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Rizzy.Configuration;
using Rizzy.Htmx;
using Rizzy.Http;

namespace Rizzy.Tests.Htmx4Coverage;

/// <summary>Extension catalog, allow-list sync and &lt;RzScripts /&gt;.</summary>
public class ExtensionTests : BunitContext
{
    private sealed class FixedNonce(string nonce) : IRizzyNonceProvider
    {
        public string GetNonce() => nonce;
    }

    [Fact]
    public void Catalog_lists_every_official_extension_once_with_its_registered_name()
    {
        HtmxExtension.Official.Should().HaveCount(17);
        HtmxExtension.Official.Select(e => e.Name).Should().OnlyHaveUniqueItems();
        HtmxExtension.Sse.Name.Should().Be("sse");
        HtmxExtension.Sse.FileName.Should().Be("hx-sse");
        HtmxExtension.Htmx2Compat.Name.Should().Be("compat");
        HtmxExtension.Rizzy.Select(e => e.Name).Should().Equal("rizzy-antiforgery", "rizzy-nonce", "rizzy-streaming", "rizzy-confirm");
        HtmxExtension.Rizzy.Should().OnlyContain(e => e.IsBundledWithRizzy);
        HtmxExtension.Official.Should().NotContain(e => e.IsBundledWithRizzy);
    }

    [Fact]
    public void UseExtensions_adds_once_and_ignores_rizzys_bundled_extensions()
    {
        var config = new HtmxConfig()
            .UseExtensions(HtmxExtension.Sse, HtmxExtension.Preload)
            .UseExtensions(HtmxExtension.Sse, HtmxExtension.RizzyNonce);

        config.EnabledExtensions.Should().Equal(HtmxExtension.Sse, HtmxExtension.Preload);
    }

    [Fact]
    public void A_copy_made_with_with_does_not_share_the_extension_list()
    {
        var original = new HtmxConfig().UseExtensions(HtmxExtension.Sse);
        var copy = original with { };
        copy.UseExtensions(HtmxExtension.Ws);

        original.EnabledExtensions.Should().Equal(HtmxExtension.Sse);
        copy.EnabledExtensions.Should().Equal(HtmxExtension.Sse, HtmxExtension.Ws);
    }

    [Fact]
    public void No_allow_list_is_written_unless_one_is_in_use()
        => new HtmxConfig().UseExtensions(HtmxExtension.Sse).ResolveExtensionAllowList().Should().BeNull();

    [Fact]
    public void A_user_allow_list_always_keeps_rizzys_extensions_and_the_enabled_ones()
    {
        var config = new HtmxConfig { Extensions = "my-ext" }.UseExtensions(HtmxExtension.Sse);

        config.ResolveExtensionAllowList(HtmxExtension.Rizzy.Select(e => e.Name)).Should()
            .Be("my-ext, rizzy-antiforgery, rizzy-nonce, rizzy-streaming, rizzy-confirm, sse");
        config.ResolveExtensionAllowList().Should().Be("my-ext, sse", "Rizzy.Htmx alone adds no Rizzy names");
    }

    [Fact]
    public void RestrictExtensions_allows_only_known_extensions()
    {
        var config = new HtmxConfig { RestrictExtensions = true }
            .UseExtensions(HtmxExtension.Preload, HtmxExtension.Custom("my-ext", "/js/my-ext.js"));

        config.ResolveExtensionAllowList(HtmxExtension.Rizzy.Select(e => e.Name)).Should()
            .Be("rizzy-antiforgery, rizzy-nonce, rizzy-streaming, rizzy-confirm, preload, my-ext");
    }

    [Fact]
    public void Set_writes_extension_settings_into_the_meta_config_with_their_own_key()
    {
        var json = new HtmxConfig()
            .Set("historyCache", new { Size = 20, RefreshOnMiss = true })
            .Set("boostBrowserIndicator", true)
            .Serialize();

        using var doc = JsonDocument.Parse(json);
        doc.RootElement.GetProperty("historyCache").GetProperty("size").GetInt32().Should().Be(20);
        doc.RootElement.GetProperty("historyCache").GetProperty("refreshOnMiss").GetBoolean().Should().BeTrue();
        doc.RootElement.GetProperty("boostBrowserIndicator").GetBoolean().Should().BeTrue();
    }

    private IRenderedComponent<T> RenderWith<T>(Action<HtmxConfig> configure, Action<RizzyConfig>? rizzy = null) where T : Microsoft.AspNetCore.Components.IComponent
    {
        Services.AddAntiforgery();
        Services.Configure<RizzyConfig>(c => { c.AntiforgeryStrategy = AntiforgeryStrategy.None; rizzy?.Invoke(c); });
        Services.AddScoped<IRizzyNonceProvider>(_ => new FixedNonce("n0nce"));
        Services.Configure(configure);
        var accessor = new MockHttpContextAccessor(Services);
        Services.AddSingleton<IHttpContextAccessor>(accessor);
        return Render<T>(p => p.AddCascadingValue(accessor.HttpContext!));
    }

    [Fact]
    public void Head_outlet_writes_the_resolved_allow_list_and_the_expected_extensions()
    {
        var cut = RenderWith<HtmxConfigHeadOutlet>(c =>
        {
            c.RestrictExtensions = true;
            c.UseExtensions(HtmxExtension.Sse);
        });

        using var doc = JsonDocument.Parse(cut.Find("meta").GetAttribute("content")!);
        doc.RootElement.GetProperty("extensions").GetString().Should()
            .Be("rizzy-antiforgery, rizzy-nonce, rizzy-streaming, rizzy-confirm, sse");
        doc.RootElement.GetProperty("rizzyExtensions").EnumerateArray().Select(e => e.GetString()).Should().Equal("sse");
    }

    [Fact]
    public void RzScripts_loads_htmx_then_rizzy_then_extensions_with_integrity_and_nonce()
    {
        var cut = RenderWith<RzScripts>(c => c.UseExtensions(
            HtmxExtension.Sse, HtmxExtension.Custom("my-ext", "/js/my-ext.js")));

        var scripts = cut.FindAll("script");
        scripts.Select(s => s.GetAttribute("src")).Should().Equal(
            $"https://cdn.jsdelivr.net/npm/htmx.org@{HtmxAssets.Version}/dist/htmx.min.js",
            "_content/Rizzy/js/rizzy.js",
            $"https://cdn.jsdelivr.net/npm/htmx.org@{HtmxAssets.Version}/dist/ext/hx-sse.min.js",
            "/js/my-ext.js");

        scripts.Should().OnlyContain(s => s.GetAttribute("nonce") == "n0nce");
        scripts[0].GetAttribute("integrity").Should().Be(HtmxAssets.Integrity["htmx.min.js"]);
        scripts[0].GetAttribute("crossorigin").Should().Be("anonymous");
        scripts[1].HasAttribute("integrity").Should().BeFalse();
        scripts[2].GetAttribute("integrity").Should().Be(HtmxAssets.Integrity["ext/hx-sse.min.js"]);
        scripts[3].HasAttribute("integrity").Should().BeFalse();
    }

    [Fact]
    public void RzScripts_can_serve_htmx_locally_unminified_without_integrity_or_skip_it()
    {
        var cut = RenderWith<RzScripts>(c =>
        {
            c.Scripts = new HtmxScriptOptions { BaseUrl = "/lib/htmx", Minified = false, Integrity = false };
            c.UseExtensions(HtmxExtension.Head);
        });

        cut.FindAll("script").Select(s => s.GetAttribute("src")).Should()
            .Equal("/lib/htmx/htmx.js", "_content/Rizzy/js/rizzy.js", "/lib/htmx/ext/hx-head.js");
        cut.FindAll("script[integrity]").Should().BeEmpty();
    }

    [Fact]
    public void Integrity_is_automatic_for_the_cdn_and_opt_in_for_your_own_copy()
    {
        new HtmxScriptOptions().UsesIntegrity.Should().BeTrue();
        new HtmxScriptOptions { BaseUrl = HtmxAssets.CdnBaseUrl.TrimEnd('/') }.UsesIntegrity.Should().BeTrue();
        new HtmxScriptOptions { BaseUrl = "/lib/htmx/" }.UsesIntegrity.Should().BeFalse("a self-hosted copy may be another version");
        new HtmxScriptOptions { BaseUrl = "/lib/htmx/", Integrity = true }.IntegrityFor("htmx").Should().Be(HtmxAssets.Integrity["htmx.min.js"]);
        new HtmxScriptOptions { Integrity = false }.IntegrityFor("htmx").Should().BeNull();
    }

    [Fact]
    public void RzScripts_loads_the_client_script_from_RizzyConfig()
    {
        var cut = RenderWith<RzScripts>(c => c.Scripts = new HtmxScriptOptions { IncludeHtmx = false },
            r => r.ClientScriptUrl = "/js/rizzy.custom.js");
        cut.FindAll("script").Select(s => s.GetAttribute("src")).Should().Equal("/js/rizzy.custom.js");
    }

    [Fact]
    public void Set_rejects_keys_that_a_property_already_writes()
    {
        FluentActions.Invoking(() => new HtmxConfig().Set("mode", "cors"))
            .Should().Throw<ArgumentException>().WithMessage("*HtmxConfig.Mode*");
        FluentActions.Invoking(() => new HtmxConfig().Set("Extensions", "x"))
            .Should().Throw<ArgumentException>().WithMessage("*HtmxConfig.Extensions*");
        FluentActions.Invoking(() => new HtmxConfig().Set("rizzyVersion", "1"))
            .Should().Throw<ArgumentException>();
        new HtmxConfig().Set("sse", new { reconnect = true }).AdditionalSettings.Should().ContainKey("sse");
    }

    [Fact]
    public void RzScripts_without_htmx_loads_only_rizzy_and_extensions()
    {
        var cut = RenderWith<RzScripts>(c => c.Scripts = new HtmxScriptOptions { IncludeHtmx = false });
        cut.FindAll("script").Select(s => s.GetAttribute("src")).Should().Equal("_content/Rizzy/js/rizzy.js");
    }

    [Fact]
    public void Every_official_extension_has_an_integrity_hash_for_both_builds()
    {
        foreach (var extension in HtmxExtension.Official)
        {
            HtmxAssets.Integrity.Should().ContainKey($"ext/{extension.FileName}.min.js");
            HtmxAssets.Integrity.Should().ContainKey($"ext/{extension.FileName}.js");
        }
    }
}
