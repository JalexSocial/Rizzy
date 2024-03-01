﻿using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Rizzy.Components;
using Rizzy.Configuration.Htmx;
using Rizzy.Configuration.Htmx.Enum;
using Rizzy.FluentAssertions;

namespace Rizzy.Configuration;

public class HtmxConfigHeadOutletTest : TestContext
{
    [Fact]
    public void HtmxConfig_serializer()
    {
        var config = new HtmxConfig
        {
            AddedClass = "added-class",
            AllowEval = true,
            AllowScriptTags = true,
            AttributesToSettle = ["attr1", "attr2"],
            DefaultFocusScroll = true,
            DefaultSettleDelay = TimeSpan.FromHours(1),
            DefaultSwapDelay = TimeSpan.FromMinutes(1),
            DefaultSwapStyle = SwapStyle.BeforeBegin,
            DisableSelector = "disable-selector",
            GetCacheBusterParam = true,
            GlobalViewTransitions = true,
            HistoryCacheSize = 1234,
            HistoryEnabled = true,
            IgnoreTitle = true,
            IncludeIndicatorStyles = true,
            IndicatorClass = "indicator-class",
            InlineScriptNonce = "inline-script-nonce",
            MethodsThatUseUrlParams = ["GET", "POST", "DELETE"],
            RefreshOnHistoryMiss = true,
            RequestClass = "request-class",
            ScrollBehavior = ScrollBehavior.Smooth,
            ScrollIntoViewOnBoost = true,
            SelfRequestsOnly = true,
            SettlingClass = "settling-class",
            SwappingClass = "swapping-class",
            Timeout = TimeSpan.FromSeconds(30),
            UseTemplateFragments = true,
            WithCredentials = true,
            WsBinaryType = "ws-binary-type",
            WsReconnectDelay = "full-jitter",
        };
        Services.AddSingleton(config);

        var cut = RenderComponent<HtmxConfigHeadOutlet>();

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
                "defaultSwapStyle": "beforeBegin",
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
