using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Rizzy.FluentAssertions;
using Rizzy.Htmx;

namespace Rizzy.Tests.Regression;

/// <summary>
/// htmx 4 response headers and request parsing (RIZZY-014, RIZZY-015, RIZZY-016, RIZZY-017).
/// </summary>
public class PhaseBHeaderTests
{
    private static HttpContext HtmxContext(Action<IHeaderDictionary>? headers = null)
    {
        var context = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection().AddOptions().BuildServiceProvider(),
        };
        context.Request.Headers[HtmxRequestHeaderNames.HtmxRequest] = "true";
        headers?.Invoke(context.Request.Headers);
        return context;
    }

    private static string Header(HttpContext context, string name) => context.Response.Headers[name].ToString();

    // ---- RIZZY-014: HX-Location matches htmx.ajax options ---------------------------------------

    [Fact]
    public void Location_serializes_values_and_headers_as_objects_with_exact_keys()
    {
        var context = HtmxContext();

        context.Response.Htmx().Location(new LocationTarget
        {
            Path = "/items",
            Target = "#main",
            Swap = "outerHTML",
            Values = new Dictionary<string, string> { ["Page"] = "2" },
            Headers = new Dictionary<string, string> { ["X-Custom-Header"] = "1" },
            SelectOob = "#nav",
            Push = "false",
            Replace = "/items?page=2",
            Transition = true,
        });

        Header(context, HtmxResponseHeaderNames.Location).Should().BeJsonSemanticallyEqualTo("""
            {
              "path": "/items", "target": "#main", "swap": "outerHTML",
              "values": { "Page": "2" }, "headers": { "X-Custom-Header": "1" },
              "selectOOB": "#nav", "push": "false", "replace": "/items?page=2", "transition": true
            }
            """);
    }

[Fact]
    public void Location_has_no_htmx2_handler_option()
        => typeof(AjaxContext).GetProperty("Handler").Should().BeNull();

        // ---- RIZZY-015: HX-Trigger is valid JSON with defined merge semantics ------------------------

    [Fact]
    public void Trigger_names_with_quotes_produce_valid_json()
    {
        var context = HtmxContext();
        context.Response.Htmx().Trigger("say \"hi\"", new { text = "a\"b" });

        using var json = JsonDocument.Parse(Header(context, HtmxResponseHeaderNames.Trigger));
        json.RootElement.GetProperty("say \"hi\"").GetProperty("text").GetString().Should().Be("a\"b");
    }

    [Fact]
    public void Trigger_header_is_ascii_even_for_non_ascii_detail()
    {
        var context = HtmxContext();
        context.Response.Htmx().Trigger("toast", new { message = "Grüße ✓" });

        var header = Header(context, HtmxResponseHeaderNames.Trigger);
        header.Should().MatchRegex("^[\\x20-\\x7E]+$");
        JsonDocument.Parse(header).RootElement.GetProperty("toast").GetProperty("message").GetString().Should().Be("Grüße ✓");
    }

    [Fact]
    public void Trigger_same_event_again_replaces_its_detail_and_keeps_order()
    {
        var context = HtmxContext();
        var response = context.Response.Htmx();

        response.Trigger("first");
        response.Trigger("second", new { n = 1 });
        response.Trigger("second", new { n = 2 });

        Header(context, HtmxResponseHeaderNames.Trigger).Should().Be("""{"first":null,"second":{"n":2}}""");
    }

    [Fact]
    public void Trigger_event_names_are_case_sensitive()
    {
        var context = HtmxContext();
        var response = context.Response.Htmx();

        response.Trigger("Saved");
        response.Trigger("saved");

        Header(context, HtmxResponseHeaderNames.Trigger).Should().Be("Saved,saved");
    }

    [Fact]
    public void TriggerOn_adds_a_target_to_object_and_primitive_details()
    {
        var context = HtmxContext();
        var response = context.Response.Htmx();

        response.TriggerOn("refresh", "#cart");
        response.TriggerOn("update", "#list", new { id = 5 });
        response.TriggerOn("count", "#badge", 3);

        Header(context, HtmxResponseHeaderNames.Trigger).Should().BeJsonSemanticallyEqualTo("""
            {
              "refresh": { "target": "#cart" },
              "update": { "id": 5, "target": "#list" },
              "count": { "value": 3, "target": "#badge" }
            }
            """);
    }

    [Fact]
    public void Trigger_detail_uses_app_json_options_camel_case_by_default()
    {
        var context = HtmxContext();
        context.Response.Htmx().Trigger("saved", new { ItemId = 7 });

        Header(context, HtmxResponseHeaderNames.Trigger).Should().Be("""{"saved":{"itemId":7}}""");
    }

    // ---- RIZZY-016: URLs in headers are ASCII ---------------------------------------------------

    [Fact]
    public void Non_ascii_uri_is_percent_encoded()
    {
        var context = HtmxContext();
        var response = context.Response.Htmx();

        response.PushUrl(new Uri("/produkte/größe?q=café", UriKind.Relative));
        response.ReplaceUrl(new Uri("https://example.com/naïve path"));

        Header(context, HtmxResponseHeaderNames.PushUrl).Should().Be("/produkte/gr%C3%B6%C3%9Fe?q=caf%C3%A9");
        Header(context, HtmxResponseHeaderNames.ReplaceUrl).Should().Be("https://example.com/na%C3%AFve%20path");
    }

    [Fact]
    public void Idn_host_uses_punycode_and_string_urls_are_encoded()
    {
        var context = HtmxContext();
        var response = context.Response.Htmx();

        response.Redirect(new Uri("https://bücher.example/ä"));
        response.Location("/suche?q=über");

        Header(context, HtmxResponseHeaderNames.Redirect).Should().Be("https://xn--bcher-kva.example/%C3%A4");
        Header(context, HtmxResponseHeaderNames.Location).Should().Be("/suche?q=%C3%BCber");
    }

    [Fact]
    public void Ascii_urls_are_unchanged()
    {
        var context = HtmxContext();
        context.Response.Htmx().PushUrl("/a/b?c=d%20e#f");

        Header(context, HtmxResponseHeaderNames.PushUrl).Should().Be("/a/b?c=d%20e#f");
    }

    // ---- RIZZY-017: request parsing -------------------------------------------------------------

    [Theory]
    [InlineData("div", null)]
    [InlineData("div#main", "main")]
    [InlineData("#legacy", "legacy")]
    [InlineData("li#item%20one", "item one")]
    [InlineData("div#", null)]
    public void TargetId_parses_htmx4_identifiers(string header, string? expected)
    {
        var context = HtmxContext(h => h[HtmxRequestHeaderNames.Target] = header);
        context.Request.Htmx().TargetId.Should().Be(expected);
    }

    [Fact]
    public void SourceId_is_null_without_an_id()
    {
        var context = HtmxContext(h => h[HtmxRequestHeaderNames.Source] = "button");
        context.Request.Htmx().SourceId.Should().BeNull();
    }

    [Theory]
    [InlineData(true, "partial", false, true)]
    [InlineData(true, "full", false, false)]
    [InlineData(true, null, false, true)]     // htmx 2 client: no request type header
    [InlineData(true, "partial", true, false)] // history restore
    [InlineData(false, "full", true, false)]   // htmx 4 restore omits HX-Request
    [InlineData(false, null, false, false)]    // plain browser request
    public void IsPartial_follows_request_type_and_history_restore(bool hxRequest, string? requestType, bool restore, bool expected)
    {
        var context = new DefaultHttpContext();
        if (hxRequest) context.Request.Headers[HtmxRequestHeaderNames.HtmxRequest] = "true";
        if (requestType != null) context.Request.Headers[HtmxRequestHeaderNames.RequestType] = requestType;
        if (restore) context.Request.Headers[HtmxRequestHeaderNames.HistoryRestoreRequest] = "true";

        var request = context.Request.Htmx();
        request.IsPartial.Should().Be(expected);
        request.IsFullPage.Should().Be(!expected);
    }

    [Fact]
    public void Request_type_is_parsed()
    {
        HtmxContext(h => h[HtmxRequestHeaderNames.RequestType] = "full").Request.Htmx().Type.Should().Be(HtmxRequestType.Full);
        HtmxContext(h => h[HtmxRequestHeaderNames.RequestType] = "partial").Request.Htmx().Type.Should().Be(HtmxRequestType.Partial);
        HtmxContext().Request.Htmx().Type.Should().Be(HtmxRequestType.None);
    }

    [Fact]
    public void HtmxContext_reuses_the_cached_request()
    {
        var context = HtmxContext();
        var cached = context.Request.Htmx();

        new Rizzy.Htmx.HtmxContext(context).Request.Should().BeSameAs(cached);
    }
}
