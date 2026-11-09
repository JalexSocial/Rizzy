using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rizzy;

namespace Rizzy.Benchmarks;

/// <summary>
/// Full request → response through a real ASP.NET Core pipeline (TestServer, in memory), so the
/// numbers include routing, layout selection, the htmx config meta tag, antiforgery tokens and
/// HTML rendering. <see cref="PlainRazorComponentResult"/> is the framework baseline without Rizzy.
/// </summary>
[MemoryDiagnoser]
public class RenderingBenchmarks
{
    private WebApplication _app = default!;
    private HttpClient _client = default!;
    private FormUrlEncodedContent? _form;

    [GlobalSetup]
    public async Task Setup()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Production" });
        builder.WebHost.UseTestServer();
        builder.Logging.ClearProviders();
        builder.Services.AddRizzy(config =>
        {
            config.RootComponent = typeof(HtmxApp<BenchRootLayout>);
            config.DefaultLayout = typeof(HtmxLayout<BenchMainLayout>);
        });

        var app = builder.Build();
        app.UseRouting();
        app.UseAntiforgery();
        app.UseRizzy();

        app.MapGet("/plain", () => new RazorComponentResult<BenchPage>(new { Rows = 20 }));
        app.MapGet("/view", (IRizzyService rizzy) => rizzy.View<BenchPage>(new { Rows = 20 }));
        app.MapGet("/partial", (IRizzyService rizzy) => rizzy.PartialView<BenchPage>(new { Rows = 20 }));
        app.MapPost("/post", (IRizzyService rizzy) => rizzy.PartialView<BenchFormPage>()).DisableAntiforgery();

        await app.StartAsync();
        _app = app;
        _client = app.GetTestClient();
    }

    [GlobalCleanup]
    public async Task Cleanup()
    {
        _client.Dispose();
        await _app.DisposeAsync();
    }

    private async Task<int> Send(HttpRequestMessage request)
    {
        using var response = await _client.SendAsync(request);
        var body = await response.Content.ReadAsByteArrayAsync();
        return body.Length;
    }

    [Benchmark(Baseline = true)]
    public Task<int> PlainRazorComponentResult() => Send(new HttpRequestMessage(HttpMethod.Get, "/plain"));

    [Benchmark(Description = "View<T>, full page")]
    public Task<int> ViewFullPage() => Send(new HttpRequestMessage(HttpMethod.Get, "/view"));

    [Benchmark(Description = "View<T>, htmx request")]
    public Task<int> ViewHtmxRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/view");
        request.Headers.Add("HX-Request", "true");
        return Send(request);
    }

    [Benchmark(Description = "PartialView<T>, htmx request")]
    public Task<int> PartialViewHtmxRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/partial");
        request.Headers.Add("HX-Request", "true");
        return Send(request);
    }

    [Benchmark(Description = "PartialView<T>, form POST")]
    public Task<int> PartialViewFormPost()
    {
        _form?.Dispose();
        _form = new FormUrlEncodedContent([new("_handler", "bench"), new("Name", "Ada")]);
        var request = new HttpRequestMessage(HttpMethod.Post, "/post") { Content = _form };
        request.Headers.Add("HX-Request", "true");
        return Send(request);
    }
}

public sealed class BenchRootLayout : LayoutComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AddMarkupContent(0, "<!DOCTYPE html>");
        builder.OpenElement(1, "html");
        builder.OpenElement(2, "head");
        builder.OpenComponent<RzHeadOutlet>(3);
        builder.CloseComponent();
        builder.CloseElement();
        builder.OpenElement(4, "body");
        builder.AddContent(5, Body);
        builder.CloseElement();
        builder.CloseElement();
    }
}

public sealed class BenchMainLayout : LayoutComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "main");
        builder.AddContent(1, Body);
        builder.CloseElement();
    }
}

/// <summary>A table of <see cref="Rows"/> rows: a typical list fragment.</summary>
public sealed class BenchPage : ComponentBase
{
    [Parameter] public int Rows { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "table");
        for (var i = 0; i < Rows; i++)
        {
            builder.OpenElement(1, "tr");
            builder.SetKey(i);
            builder.OpenElement(2, "td");
            builder.AddContent(3, i);
            builder.CloseElement();
            builder.OpenElement(4, "td");
            builder.AddContent(5, "Row ");
            builder.AddContent(6, i);
            builder.CloseElement();
            builder.CloseElement();
        }
        builder.CloseElement();
    }
}

/// <summary>Binds a posted form through [SupplyParameterFromForm].</summary>
public sealed class BenchFormPage : ComponentBase
{
    [SupplyParameterFromForm(FormName = "bench")] public string? Name { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.AddContent(0, Name);
}
