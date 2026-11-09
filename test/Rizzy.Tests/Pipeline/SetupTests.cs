using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rizzy.Configuration;
using Rizzy.Htmx;

namespace Rizzy.Tests.Pipeline;

/// <summary>Setup defaults and diagnostics (RIZZY-026, RIZZY-030, RM-C3).</summary>
public class SetupTests
{
    private static WebApplication BuildApp(Action<IServiceCollection> configure)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Testing" });
        builder.WebHost.UseTestServer();
        builder.Logging.ClearProviders();
        configure(builder.Services);

        var app = builder.Build();
        app.UseRouting();
        app.UseAntiforgery();
        app.UseRizzy();
        app.MapGet("/page", (IRizzyService rizzy) => rizzy.View<TestPage>(new { Message = "only AddRizzy" }));
        return app;
    }

    [Fact]
    public async Task AddRizzy_alone_registers_htmx_antiforgery_and_razor_components()
    {
        await using var app = BuildApp(s => s.AddRizzy(c =>
        {
            c.RootComponent = typeof(HtmxApp<TestRootLayout>);
            c.DefaultLayout = typeof(HtmxLayout<TestMainLayout>);
        }));
        await app.StartAsync();

        var html = await app.GetTestClient().GetStringAsync("/page");

        html.Should().Contain("only AddRizzy");
        html.Should().Contain("\"headerName\":\"RequestVerificationToken\"", "AddRizzy must configure the htmx antiforgery options");
    }

    [Fact]
    public void Repeated_registration_does_not_duplicate_services_and_applies_every_configuration()
    {
        var services = new ServiceCollection().AddLogging();
        services.AddRazorComponents();
        services.AddRizzy(c => c.AntiforgeryStrategy = AntiforgeryStrategy.None);
        services.AddHtmx(c => c.LogAll = true);
        services.AddRizzy(c => c.DefaultLayout = typeof(TestMainLayout));
        services.AddRazorComponents();

        services.Count(d => d.ServiceType == typeof(IRizzyService)).Should().Be(1);
        services.Count(d => d.ServiceType == typeof(IHtmxSwapService)).Should().Be(1);

        using var provider = services.BuildServiceProvider();
        var rizzy = provider.GetRequiredService<IOptions<RizzyConfig>>().Value;
        rizzy.AntiforgeryStrategy.Should().Be(AntiforgeryStrategy.None);
        rizzy.DefaultLayout.Should().Be(typeof(TestMainLayout));
        provider.GetRequiredService<IOptions<HtmxConfig>>().Value.LogAll.Should().BeTrue();
    }

    [Fact]
    public async Task Missing_root_component_fails_at_startup()
    {
        await using var app = BuildApp(s => s.AddRizzy(c => c.RootComponent = null));

        var start = () => app.StartAsync();

        (await start.Should().ThrowAsync<OptionsValidationException>()).WithMessage("*RootComponent*");
    }

    [Fact]
    public void Invalid_layout_types_throw_ArgumentException()
    {
        var config = new RizzyConfig();

        config.Invoking(c => c.DefaultLayout = typeof(string)).Should().Throw<ArgumentException>().WithParameterName("DefaultLayout");
        config.Invoking(c => c.RootComponent = typeof(TestMainLayout)).Should().Throw<ArgumentException>().WithParameterName("RootComponent");
    }

    [Fact]
    public async Task Default_root_component_logs_a_warning()
    {
        var logs = new List<string>();
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Testing" });
        builder.WebHost.UseTestServer();
        builder.Logging.ClearProviders().AddProvider(new ListLoggerProvider(logs));
        builder.Services.AddRizzy();
        await using var app = builder.Build();

        await app.StartAsync();

        logs.Should().ContainSingle(l => l.Contains("HtmxApp<EmptyLayout>"));
    }

    private sealed class ListLoggerProvider(List<string> logs) : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new ListLogger(logs);
        public void Dispose() { }

        private sealed class ListLogger(List<string> logs) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
            public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Warning;
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                if (IsEnabled(logLevel)) lock (logs) logs.Add(formatter(state, exception));
            }
        }
    }
}
