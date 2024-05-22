using Bunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Rizzy.Antiforgery;
using Rizzy.Components;
using Rizzy.Configuration.Htmx;
using Rizzy.FluentAssertions;
using Rizzy.Framework.Services;
using Rizzy.Http;

namespace Rizzy.Configuration;

public class HtmxConfigHeadOutletTest : TestContext
{
    [Fact]
    public void HtmxConfig_serializer()
    {
        Services.AddScoped<RzViewContext>();
        Services.AddScoped<IUrlHelper>(provider => null!);
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

        Services.Configure<HtmxConfig>(config =>
        {
            config.AddedClass = "added-class";
            config.AllowEval = true;
            config.AllowScriptTags = true;
            config.AttributesToSettle = ["attr1", "attr2"];
            config.DefaultFocusScroll = true;
            config.DefaultSettleDelay = TimeSpan.FromHours(1);
            config.DefaultSwapDelay = TimeSpan.FromMinutes(1);
            config.DefaultSwapStyle = SwapStyle.beforebegin;
            config.DisableSelector = "disable-selector";
            config.GetCacheBusterParam = true;
            config.GlobalViewTransitions = true;
            config.HistoryCacheSize = 1234;
            config.HistoryEnabled = true;
            config.IgnoreTitle = true;
            config.IncludeIndicatorStyles = true;
            config.IndicatorClass = "indicator-class";
            config.InlineScriptNonce = "inline-script-nonce";
            config.MethodsThatUseUrlParams = ["GET", "POST", "DELETE"];
            config.RefreshOnHistoryMiss = true;
            config.RequestClass = "request-class";
            config.ScrollBehavior = ScrollBehavior.smooth;
            config.ScrollIntoViewOnBoost = true;
            config.SelfRequestsOnly = true;
            config.SettlingClass = "settling-class";
            config.SwappingClass = "swapping-class";
            config.Timeout = TimeSpan.FromSeconds(30);
            config.UseTemplateFragments = true;
            config.WithCredentials = true;
            config.WsBinaryType = "ws-binary-type";
            config.WsReconnectDelay = "full-jitter";
        });

        var accessor = new MockHttpContextAccessor(Services);
        Services.AddSingleton<IHttpContextAccessor>(accessor);

        var cut = RenderComponent<HtmxConfigHeadOutlet>(p => p.AddCascadingValue(accessor.HttpContext!));

        var meta = cut.Find("meta");
        meta.GetAttribute("name").Should().Be("htmx-config");
        meta.GetAttribute("content").Should().BeJsonSemanticallyEqualTo("""
            {
                "addedClass": "added-class",
                "allowEval": true,
                "allowScriptTags": true,
                "attributesToSettle": [
                    "attr1",
                    "attr2"
                ],
                "defaultFocusScroll": true,
                "defaultSwapStyle": "beforebegin",
                "defaultSwapDelay": 60000,
                "defaultSettleDelay": 3600000,
                "disableSelector": "disable-selector",
                "getCacheBusterParam": true,
                "globalViewTransitions": true,
                "historyCacheSize": 1234,
                "historyEnabled": true,
                "ignoreTitle": true,
                "includeIndicatorStyles": true,
                "indicatorClass": "indicator-class",
                "inlineScriptNonce": "inline-script-nonce",
                "methodsThatUseUrlParams": [
                    "GET",
                    "POST",
                    "DELETE"
                ],
                "refreshOnHistoryMiss": true,
                "requestClass": "request-class",
                "scrollBehavior": "smooth",
                "scrollIntoViewOnBoost": true,
                "selfRequestsOnly": true,
                "settlingClass": "settling-class",
                "swappingClass": "swapping-class",
                "timeout": 30000,
                "useTemplateFragments": true,
                "withCredentials": true,
                "wsBinaryType": "ws-binary-type",
                "wsReconnectDelay": "full-jitter"
            }
            """);
    }
}
