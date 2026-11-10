using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rizzy.Htmx;

namespace Rizzy.Tests.Htmx4Coverage;

// HxAttributes.DefaultMetaCharacter is process-wide: keep every test that reads or sets it in one
// non-parallel collection.
[CollectionDefinition(nameof(HxAttributesCollection), DisableParallelization = true)]
public sealed class HxAttributesCollection;

/// <summary>The Hx attribute builder (one method per htmx 4 and extension attribute).</summary>
[Collection(nameof(HxAttributesCollection))]
public class HxAttributesTests : BunitContext
{
    private static Dictionary<string, object> Render(HxAttributes attributes) => attributes.ToDictionary(a => a.Key, a => a.Value);

    [Fact]
    public void Request_targeting_and_swap_attributes()
    {
        var attrs = Render(Hx.Post("/save")
            .Target(HxTarget.Closest("form"))
            .Swap(SwapStyle.outerHTML.Transition(true))
            .Trigger(Trigger.OnEvent("click").Once())
            .Select("#main")
            .SelectOob("#nav, #alert:afterbegin")
            .Include(HxTarget.Find("input"))
            .Indicator("#spinner")
            .Disable("find button")
            .PushUrl()
            .ReplaceUrl("/edited")
            .Boost(false));

        attrs.Should().BeEquivalentTo(new Dictionary<string, object>
        {
            ["hx-post"] = "/save",
            ["hx-target"] = "closest form",
            ["hx-swap"] = "outerHTML transition:true",
            ["hx-trigger"] = "click once",
            ["hx-select"] = "#main",
            ["hx-select-oob"] = "#nav, #alert:afterbegin",
            ["hx-include"] = "find input",
            ["hx-indicator"] = "#spinner",
            ["hx-disable"] = "find button",
            ["hx-push-url"] = "true",
            ["hx-replace-url"] = "/edited",
            ["hx-boost"] = "false",
        });
    }

    [Fact]
    public void Every_verb_and_action_method()
    {
        Render(Hx.Get("/a")).Should().ContainKey("hx-get");
        Render(Hx.Put("/a")).Should().ContainKey("hx-put");
        Render(Hx.Patch("/a")).Should().ContainKey("hx-patch");
        Render(Hx.Delete("/a")).Should().ContainKey("hx-delete");
        Render(Hx.Query("/a")).Should().ContainKey("hx-query");
        Render(Hx.Action("/a").Method("POST")).Should().BeEquivalentTo(new Dictionary<string, object> { ["hx-action"] = "/a", ["hx-method"] = "POST" });
    }

    [Fact]
    public void Json_values_are_encoded_and_camel_cased()
    {
        var attrs = Render(Hx.Vals(new { ItemId = 5, Note = "it's <b>" })
            .Headers(new Dictionary<string, string> { ["X-Mode"] = "fast" })
            .Config(new { Timeout = 5000 })
            .Confirm(new HxConfirm { Title = "Delete?", ShowCancelButton = true })
            .Boost(new { Target = "#main" }));

        attrs["hx-vals"].Should().Be("{\"itemId\":5,\"note\":\"it\\u0027s \\u003Cb\\u003E\"}");
        attrs["hx-headers"].Should().Be("{\"X-Mode\":\"fast\"}");
        attrs["hx-config"].Should().Be("{\"timeout\":5000}");
        attrs["hx-confirm"].Should().Be("{\"title\":\"Delete?\",\"showCancelButton\":true}");
        attrs["hx-boost"].Should().Be("{\"target\":\"#main\"}");
    }

    [Theory]
    [InlineData(HxSyncStrategy.Drop, "closest form:drop")]
    [InlineData(HxSyncStrategy.Abort, "closest form:abort")]
    [InlineData(HxSyncStrategy.Replace, "closest form:replace")]
    [InlineData(HxSyncStrategy.QueueFirst, "closest form:queue first")]
    [InlineData(HxSyncStrategy.QueueLast, "closest form:queue last")]
    [InlineData(HxSyncStrategy.QueueAll, "closest form:queue all")]
    public void Sync_strategies(HxSyncStrategy strategy, string expected)
        => Render(Hx.Sync("closest form", strategy))["hx-sync"].Should().Be(expected);

    [Fact]
    public void Status_on_flags_and_validation()
    {
        var attrs = Render(Hx.Status(422, "target:#errors")
            .Status("5xx", "swap:none")
            .On(HtmxEvents.AfterSwap, "init()")
            .On(("click once", "a()"), ("blur", "b()"))
            .Ignore().Preserve().HistoryElt().MorphSkip().MorphSkipChildren()
            .Validate(false).EncodingMultipart().SwapOob(SwapStyle.afterbegin));

        attrs["hx-status:422"].Should().Be("target:#errors");
        attrs["hx-status:5xx"].Should().Be("swap:none");
        attrs["hx-on:htmx:after:swap"].Should().Be("init()");
        attrs["hx-on"].Should().Be("click once -> a(); blur -> b()");
        foreach (var flag in new[] { "hx-ignore", "hx-preserve", "hx-history-elt", "hx-morph-skip", "hx-morph-skip-children" })
            attrs[flag].Should().Be("true");
        attrs["hx-validate"].Should().Be("false");
        attrs["hx-encoding"].Should().Be("multipart/form-data");
        attrs["hx-swap-oob"].Should().Be("afterbegin");
    }

    [Fact]
    public void Extension_attributes()
    {
        var attrs = Render(Hx.SseConnect("/events").SseClose("done")
            .WsConnect("/ws").WsSend()
            .MultipartConnect("/parts").MultipartClose("end")
            .Preload(Trigger.OnEvent("mousedown"))
            .Head(HxHeadStrategy.ReEval).Prompt("Name?").Live("q('#n').value")
            .Pending("#loading-tpl").PTag("v1").Targets(".card").BrowserIndicator().Nonce("abc").History(false));

        attrs.Should().BeEquivalentTo(new Dictionary<string, object>
        {
            ["hx-sse:connect"] = "/events",
            ["hx-sse:close"] = "done",
            ["hx-ws:connect"] = "/ws",
            ["hx-ws:send"] = "true",
            ["hx-multipart:connect"] = "/parts",
            ["hx-multipart:close"] = "end",
            ["hx-preload"] = "mousedown",
            ["hx-head"] = "re-eval",
            ["hx-prompt"] = "Name?",
            ["hx-live"] = "q('#n').value",
            ["hx-pending"] = "#loading-tpl",
            ["hx-ptag"] = "v1",
            ["hx-targets"] = ".card",
            ["hx-browser-indicator"] = "true",
            ["hx-nonce"] = "abc",
            ["hx-history"] = "false",
        });
        Render(Hx.Preload())["hx-preload"].Should().Be("");
    }

    [Fact]
    public void Inherited_and_Append_modify_the_previous_attribute()
    {
        Render(Hx.Target("#main").Inherited().Include("#filters").Append())
            .Should().BeEquivalentTo(new Dictionary<string, object>
            {
                ["hx-target:inherited"] = "#main",
                ["hx-include:append"] = "#filters",
            });

        FluentActions.Invoking(() => Hx.Attributes().Inherited()).Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Inherited_applies_to_the_attribute_just_set_even_when_it_replaced_an_earlier_one()
    {
        Render(Hx.Target("#a").Swap(SwapStyle.innerHTML).Target("#b").Inherited())
            .Should().BeEquivalentTo(new Dictionary<string, object>
            {
                ["hx-target:inherited"] = "#b",
                ["hx-swap"] = "innerHTML",
            });
    }

    [Fact]
    public void Setting_an_inherited_attribute_again_replaces_it_instead_of_adding_a_second()
    {
        Render(Hx.Target("#a").Inherited().Target("#b"))
            .Should().BeEquivalentTo(new Dictionary<string, object> { ["hx-target"] = "#b" });
    }

    [Fact]
    public void Inherited_and_Append_combine()
    {
        Render(Hx.Include("#f").Inherited().Append()).Keys.Should().Equal("hx-include:inherited:append");
        Render(Hx.Include("#f").Append().Inherited()).Keys.Should().Equal("hx-include:inherited:append");
        Render(Hx.Include("#f").Inherited().Inherited()).Keys.Should().Equal("hx-include:inherited");
    }

    [Fact]
    public void Removing_an_attribute_leaves_nothing_for_Inherited_to_modify()
        => FluentActions.Invoking(() => Hx.Target("#a").Attr("hx-target", null).Inherited())
            .Should().Throw<InvalidOperationException>();

    [Fact]
    public void Calling_a_method_again_replaces_the_value_and_null_removes_it()
    {
        var attrs = Render(Hx.Target("#a").Target("#b").Attr("data-x", "1").Attr("data-x", null));
        attrs.Should().BeEquivalentTo(new Dictionary<string, object> { ["hx-target"] = "#b" });
    }

    [Fact]
    public void Meta_character_replaces_colons_in_names()
    {
        var attrs = Render(Hx.Status(422, "target:#e").On(HtmxEvents.AfterSwap, "x()").SseConnect("/e")
            .Target("#t").Inherited().WithMetaCharacter("-"));

        attrs.Keys.Should().BeEquivalentTo("hx-status-422", "hx-on-htmx-after-swap", "hx-sse-connect", "hx-target-inherited");
        attrs["hx-status-422"].Should().Be("target:#e", "values are not changed");
    }

    [Fact]
    public async Task Default_meta_character_comes_from_HtmxConfig_at_startup()
    {
        try
        {
            var builder = WebApplication.CreateBuilder();
            builder.WebHost.UseTestServer();
            builder.Logging.ClearProviders();
            builder.Services.AddHtmx(o => o.MetaCharacter = "-");
            await using var app = builder.Build();
            await app.StartAsync();

            HxAttributes.DefaultMetaCharacter.Should().Be("-");
            Render(Hx.On("click", "x()")).Should().ContainKey("hx-on-click");
            await app.StopAsync();
        }
        finally
        {
            HxAttributes.DefaultMetaCharacter = null;
        }
    }

    [Fact]
    public void Hx_has_a_static_entry_for_every_attribute_method()
    {
        var excluded = new[] { "WithMetaCharacter", "Inherited", "Append", "GetEnumerator", "ToString", "Equals", "GetHashCode", "GetType" };
        static string Signature(MethodInfo m) => m.Name + "(" + string.Join(",", m.GetParameters().Select(p => p.ParameterType.FullName)) + ")";

        var instance = typeof(HxAttributes).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.ReturnType == typeof(HxAttributes) && !excluded.Contains(m.Name))
            .Select(Signature).ToHashSet();
        var statics = typeof(Hx).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.ReturnType == typeof(HxAttributes))
            .Select(Signature).ToHashSet();

        instance.Except(statics).Should().BeEmpty("every HxAttributes method needs a matching Hx entry point");
    }

    private sealed class Splatted : ComponentBase
    {
        [Parameter] public HxAttributes Attributes { get; set; } = default!;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "button");
            builder.AddMultipleAttributes(1, Attributes);
            builder.AddContent(2, "go");
            builder.CloseElement();
        }
    }

    [Fact]
    public void Blazor_renders_splatted_attributes_including_names_with_colons()
    {
        var cut = Render<Splatted>(p => p.Add(c => c.Attributes,
            Hx.Post("/x").Status(422, "target:#e").On("click", "a()").Vals(new { id = 1 }).HistoryElt()));

        var button = cut.Find("button");
        button.GetAttribute("hx-post").Should().Be("/x");
        button.GetAttribute("hx-status:422").Should().Be("target:#e");
        button.GetAttribute("hx-on:click").Should().Be("a()");
        button.GetAttribute("hx-vals").Should().Be("{\"id\":1}");
        button.GetAttribute("hx-history-elt").Should().Be("true");
    }

    [Fact]
    public void HxTarget_extended_selectors()
    {
        HxTarget.Id("item:1").Should().Be("#item\\:1");
        HxTarget.Closest("tr").Should().Be("closest tr");
        HxTarget.Find(".x").Should().Be("find .x");
        HxTarget.FindAll(".x").Should().Be("findAll .x");
        HxTarget.Next().Should().Be("next");
        HxTarget.Next(".x").Should().Be("next .x");
        HxTarget.Previous().Should().Be("previous");
        HxTarget.Global("#x").Should().Be("global #x");
        new[] { HxTarget.This, HxTarget.Document, HxTarget.Window, HxTarget.Body, HxTarget.Host }
            .Should().Equal("this", "document", "window", "body", "host");
    }
}

/// <summary>Event name constants and extension request/response headers.</summary>
public class HtmxEventsAndExtensionHeaderTests
{
    [Fact]
    public void Event_constants()
    {
        HtmxEvents.AfterSwap.Should().Be("htmx:after:swap");
        HtmxEvents.AfterImplicitInheritance.Should().Be("htmx:after:implicitInheritance");
        HtmxEvents.Process("partial").Should().Be("htmx:process:partial");
        HtmxEvents.Sse.AfterMessage.Should().Be("htmx:sse:after:message");
        HtmxEvents.Ws.BeforeMessageOutgoing.Should().Be("htmx:ws:before:message:outgoing");
        HtmxEvents.HistoryCache.Miss.Should().Be("htmx:history:cache:miss");
        HtmxEvents.Rizzy.ServerError.Should().Be("rz:server-error");
        HtmxEvents.WithMetaCharacter(HtmxEvents.AfterSwap, "-").Should().Be("htmx-after-swap");
        HtmxEvents.WithMetaCharacter(HtmxEvents.AfterSwap, null).Should().Be("htmx:after:swap");
    }

    [Fact]
    public void Trigger_builder_accepts_event_constants()
        => Trigger.OnEvent(HtmxEvents.Sse.AfterMessage).ToString().Should().Be("htmx:sse:after:message");

    [Fact]
    public void Extension_request_headers_are_parsed()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[HtmxRequestHeaderNames.HtmxRequest] = "true";
        context.Request.Headers[HtmxRequestHeaderNames.Prompt] = Uri.EscapeDataString("Ada Lovelace");
        context.Request.Headers[HtmxRequestHeaderNames.PTag] = "v7";
        context.Request.Headers[HtmxRequestHeaderNames.LastPartId] = "42";

        var request = context.Request.Htmx();
        request.Prompt.Should().Be("Ada Lovelace");
        request.PTag.Should().Be("v7");
        request.LastPartId.Should().Be("42");
    }

    [Fact]
    public void Extension_response_headers_are_set()
    {
        var context = new DefaultHttpContext { RequestServices = new ServiceCollection().BuildServiceProvider() };
        context.Request.Headers[HtmxRequestHeaderNames.HtmxRequest] = "true";

        context.Response.Htmx().PTag("v8").Download("/files/report.pdf");

        context.Response.Headers[HtmxResponseHeaderNames.PTag].ToString().Should().Be("v8");
        context.Response.Headers[HtmxResponseHeaderNames.Download].ToString().Should().Be("/files/report.pdf");
    }
}
