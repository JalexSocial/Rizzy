using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rizzy.Framework.Mvc;
using Rizzy.Htmx;

namespace Rizzy.Tests.Pipeline;

/// <summary>
/// A real ASP.NET Core pipeline (TestServer) configured the way a Rizzy app is configured.
/// One host is shared by all tests in a class through <see cref="IClassFixture{TFixture}"/>.
/// </summary>
public sealed class RizzyTestHost : IAsyncLifetime
{
    private WebApplication? _app;

    public HttpClient Client { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Testing" });
        builder.WebHost.UseTestServer();
        builder.Logging.ClearProviders();

        builder.Services.AddRazorComponents();
        // AddControllersWithViews registers the MVC antiforgery filters used by [ValidateAntiForgeryToken].
        builder.Services.AddControllersWithViews().AddApplicationPart(typeof(RizzyTestHost).Assembly);
        builder.Services.AddRizzy(config =>
        {
            config.RootComponent = typeof(HtmxApp<TestRootLayout>);
            config.DefaultLayout = typeof(HtmxLayout<TestMainLayout>);
        });
        builder.Services.AddHtmx();

        var app = builder.Build();
        app.UseRouting();
        app.UseAntiforgery();
        app.UseRizzy();

        app.MapGet("/page", (IRizzyService rizzy) => rizzy.View<TestPage>(new { Message = "hello page" }));
        app.MapGet("/partial", (IRizzyService rizzy) => rizzy.PartialView<TestPage>(new { Message = "hello partial" }));
        app.MapGet("/swap", (IRizzyService rizzy) => rizzy.PartialView<TestSwapPage>());
        app.MapGet("/form", (IRizzyService rizzy) => rizzy.View<TestFormPage>());
        app.MapGet("/form-invalid", (IRizzyService rizzy) =>
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError(nameof(TestPerson.Name), "Server says the name is taken.");
            return rizzy.View<TestFormPage>(new { Person = new TestPerson { Name = "Bob" } }, modelState);
        });
        app.MapGet("/empty", (HttpContext ctx, IRizzyService rizzy) =>
        {
            ctx.Response.Htmx().EmptyBody();
            return rizzy.PartialView<TestPage>(new { Message = "never rendered" });
        });
        app.MapGet("/redirect", (HttpContext ctx, IRizzyService rizzy) =>
        {
            ctx.Response.Htmx().Redirect("/elsewhere");
            return rizzy.PartialView<TestPage>(new { Message = "never rendered" });
        });
        app.MapGet("/page-with-head", (IRizzyService rizzy) => rizzy.View<TestHeadContentPage>());
        app.MapGet("/stream", (IRizzyService rizzy) => rizzy.View<TestStreamingPage>());
        app.MapPost("/minimal-post", (IRizzyService rizzy) => rizzy.View<TestPostedFormPage>()).DisableAntiforgery();
        app.MapControllers();

        await app.StartAsync();
        _app = app;
        Client = app.GetTestClient();
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();
        if (_app is not null)
            await _app.DisposeAsync();
    }

    public Task<HttpResponseMessage> GetAsync(string url, params (string Name, string Value)[] headers)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        foreach (var (name, value) in headers)
            request.Headers.TryAddWithoutValidation(name, value);
        return Client.SendAsync(request);
    }

    /// <summary>Headers sent by htmx 4 for a request that targets a single element.</summary>
    public static readonly (string, string)[] PartialHtmxHeaders =
    [
        ("HX-Request", "true"),
        ("HX-Request-Type", "partial"),
        ("HX-Target", "div#content"),
        ("HX-Source", "a#link"),
    ];

    /// <summary>Headers sent by htmx 4 when it restores a page on back/forward navigation.</summary>
    public static readonly (string, string)[] HistoryRestoreHeaders =
    [
        ("HX-Request", "true"),
        ("HX-Request-Type", "full"),
        ("HX-History-Restore-Request", "true"),
    ];

    /// <summary>
    /// Headers htmx 4.0.1 actually sends on history restore: its <c>htmx.ajax</c> call replaces the
    /// request object, so only the restore header and the request type survive.
    /// </summary>
    public static readonly (string, string)[] HistoryRestoreHeadersAsSent =
    [
        ("HX-Request-Type", "full"),
        ("HX-History-Restore-Request", "true"),
    ];

    /// <summary>Headers sent by htmx 4 for a boosted link that targets the body.</summary>
    public static readonly (string, string)[] FullPageHtmxHeaders =
    [
        ("HX-Request", "true"),
        ("HX-Request-Type", "full"),
        ("HX-Boosted", "true"),
    ];
}

public sealed class TestResponseAttributeController : RzController
{
    [HttpGet("/ctrl/typed")]
    public IResult Typed() => View<TestPage>(p => p.Add(x => x.Message, "typed " + CurrentActionUrl));

    [HttpGet("/ctrl/fragments")]
    public IResult Fragments() => PartialView(
        b => b.AddMarkupContent(0, "<b id=\"f1\">one</b>"),
        b => b.AddMarkupContent(0, "<b id=\"f2\">two</b>"));

    [HttpPost("/ctrl/post")]
    [IgnoreAntiforgeryToken]
    public IResult Post() => View<TestPostedFormPage>();

    [HttpPost("/ctrl/antiforgery-validated")]
    [ValidateAntiForgeryToken]
    public IResult AntiforgeryValidated() => PartialView<TestPage>(new { Message = "accepted" });

    [HttpGet("/attr")]
    [HtmxResponse(PushUrl = "/pushed", Refresh = true, PreventBrowserCurrentUrlUpdate = true, Reswap = SwapStyle.outerHTML)]
    public IResult WithAttribute() => PartialView<TestPage>(new { Message = "attribute" });
}
