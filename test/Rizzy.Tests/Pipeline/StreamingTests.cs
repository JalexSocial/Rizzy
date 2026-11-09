using System.Text;

namespace Rizzy.Tests.Pipeline;

/// <summary>
/// Server half of streaming rendering with htmx (RIZZY-003): the response must contain a complete
/// initial document (ending in &lt;/html&gt;) that is flushed before the async work finishes, followed
/// by &lt;blazor-ssr&gt; update blocks. rizzy-streaming.js relies on exactly this shape.
/// </summary>
[Collection(nameof(StreamingTests))] // the page's gate is static: run these tests one at a time
public class StreamingTests(RizzyTestHost host) : IClassFixture<RizzyTestHost>
{
    private static async Task<string> ReadUntilAsync(Stream stream, StringBuilder buffer, string marker, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        var bytes = new byte[4096];
        while (!buffer.ToString().Contains(marker, StringComparison.Ordinal))
        {
            var read = await stream.ReadAsync(bytes, cts.Token);
            if (read == 0) break;
            buffer.Append(Encoding.UTF8.GetString(bytes, 0, read));
        }
        return buffer.ToString();
    }

    private async Task<(string Initial, string Full)> FetchAsync(params (string, string)[] headers) => await FetchAsync("/stream", "</html>", headers);

    private async Task<(string Initial, string Full)> FetchAsync(string url, string initialMarker, params (string, string)[] headers)
    {
        TestStreamingPage.NewGate();
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        foreach (var (name, value) in headers)
            request.Headers.TryAddWithoutValidation(name, value);

        using var response = await host.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        await using var stream = await response.Content.ReadAsStreamAsync();
        var buffer = new StringBuilder();

        // The initial document must arrive while the component is still waiting.
        var initial = await ReadUntilAsync(stream, buffer, initialMarker, TimeSpan.FromSeconds(5));
        TestStreamingPage.Release();

        var full = await ReadUntilAsync(stream, buffer, "</blazor-ssr>", TimeSpan.FromSeconds(5));
        return (initial, full);
    }

    [Fact]
    public async Task Partial_htmx_request_streams_a_minimal_document_then_update_blocks()
    {
        var (initial, full) = await FetchAsync(RizzyTestHost.PartialHtmxHeaders);

        initial.Should().Contain("</html>", "the initial document is flushed before the async work completes");
        initial.Should().Contain("id=\"loading\"").And.NotContain("id=\"loaded\"");
        initial.Should().NotContain("main-layout", "partial requests get no application layout");

        var afterDocument = full[(full.IndexOf("</html>", StringComparison.Ordinal) + "</html>".Length)..];
        afterDocument.Should().Contain("<blazor-ssr>").And.Contain("id=\"loaded\"");
        afterDocument.Should().Contain("<blazor-ssr-end>", "the update block patches the component markers");

        // Same shape as the fixture in packages/rizzy/tests/htmx4-integration.test.mjs: the update
        // template names the component whose <!--bl:N--> markers are in the initial document.
        var id = System.Text.RegularExpressions.Regex.Match(initial, @"<!--bl:(\d+)-->").Groups[1].Value;
        id.Should().NotBeEmpty();
        initial.Should().StartWith("<html><body>").And.Contain($"<!--/bl:{id}-->");
        afterDocument.Should().Contain($"<template blazor-component-id=\"{id}\">");
    }

    [Fact]
    public async Task Full_page_request_streams_the_layout_then_update_blocks()
    {
        var (initial, full) = await FetchAsync();

        initial.Should().Contain("main-layout").And.Contain("id=\"loading\"").And.Contain("</html>");
        full.Should().Contain("<blazor-ssr>").And.Contain("id=\"loaded\"");
    }

    [Fact]
    public async Task PartialView_streams_the_fragment_then_update_blocks()
    {
        var (initial, full) = await FetchAsync("/stream-partial", "<!--/bl:", RizzyTestHost.PartialHtmxHeaders);

        initial.Should().Contain("id=\"loading\"").And.NotContain("id=\"loaded\"");
        full.Should().Contain("<blazor-ssr>").And.Contain("id=\"loaded\"");
    }
}
