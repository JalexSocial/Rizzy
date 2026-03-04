using Bunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Rizzy;
using Rizzy.Components;
using Rizzy.FluentAssertions;
using Rizzy.Htmx;
using Rizzy.Http;

namespace Rizzy.Configuration;

public class HtmxConfigHeadOutletTest : BunitContext
{
    private sealed class TestNonceProvider(string nonce) : IRizzyNonceProvider
    {
        public string GetNonce() => nonce;
    }

    [Fact]
    public void HtmxConfig_serializer()
    {
        Services.Configure<HtmxAntiforgeryOptions>(opt =>
        {
            // Default to true, can be configured again after adding htmx if necessary
            opt.FormFieldName = "abcd";
            opt.HeaderName = "abcd";
            opt.CookieName = "HX-XSRF-TOKEN";
        });
        Services.AddAntiforgery();
        Services.Configure<RizzyConfig>(config =>
        {
            config.AntiforgeryStrategy = AntiforgeryStrategy.None;
        });
        Services.AddScoped<IRizzyNonceProvider>(_ => new TestNonceProvider("test-nonce"));

        Services.Configure<HtmxConfig>(config =>
        {
            config.DefaultFocusScroll = true;
            config.DefaultSettleDelay = TimeSpan.FromHours(1);
            config.DefaultSwap = SwapStyle.beforebegin;
            config.DefaultTimeout = TimeSpan.FromSeconds(30);
            config.History = true;
            config.ImplicitInheritance = true;
            config.IncludeIndicatorCSS = true;
            config.IndicatorClass = "indicator-class";
            config.InlineScriptNonce = "inline-script-nonce";
            config.InlineStyleNonce = "inline-style-nonce";
            config.Mode = "same-origin";
            config.NoSwap = ["204", "4xx", "5xx"];
            config.RequestClass = "request-class";
            config.Transitions = true;
        });

        var accessor = new MockHttpContextAccessor(Services);
        Services.AddSingleton<IHttpContextAccessor>(accessor);

        var cut = Render<HtmxConfigHeadOutlet>(p => p.AddCascadingValue(accessor.HttpContext!));

        var meta = cut.Find("meta");
        meta.GetAttribute("name").Should().Be("htmx-config");

        var expectedContent = $$"""
            {
                "defaultFocusScroll": true,
                "defaultSwap": "beforebegin",
                "defaultSettleDelay": 3600000,
                "defaultTimeout": 30000,
                "documentNonce": "test-nonce",
                "history": true,
                "implicitInheritance": true,
                "includeIndicatorCSS": true,
                "indicatorClass": "indicator-class",
                "inlineScriptNonce": "inline-script-nonce",
                "inlineStyleNonce": "inline-style-nonce",
                "mode": "same-origin",
                "noSwap": [
                    "204",
                    "4xx",
                    "5xx"
                ],
                "requestClass": "request-class",
                "transitions": true
            }
            """;

        meta.GetAttribute("content").Should().BeJsonSemanticallyEqualTo(expectedContent);
    }
}
