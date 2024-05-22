using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Rizzy.Antiforgery;
using Rizzy.Components;
using Rizzy.Components.Form;
using Rizzy.Components.Form.Helpers;
using Rizzy.Configuration.Htmx;

namespace Rizzy.Configuration;

public class RizzyConfigBuilder
{
    private readonly string _defaultCookieName = "HX-XSRF-TOKEN";
    private readonly IHostApplicationBuilder _builder;

    /// <summary>
    /// Configures HtmxConfig options to include the antiforgery configuration
    /// </summary>
    public class ConfigureHtmxSettings : IConfigureOptions<HtmxConfig>, IConfigureNamedOptions<HtmxConfig>
    {
        private HtmxAntiforgeryOptions _antiforgeryOptions = default!;

        public ConfigureHtmxSettings(IOptions<HtmxAntiforgeryOptions> antiforgeryOptions)
        {
            _antiforgeryOptions = antiforgeryOptions.Value;
        }

        public void Configure(string? name, HtmxConfig options)
        {
            // Do nothing for now
        }

        public void Configure(HtmxConfig options) => Configure(Options.DefaultName, options);
    }

    /// <summary>
    /// Sets up default services for Rizzy
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configBuilder"></param>
    public RizzyConfigBuilder(IHostApplicationBuilder builder, Action<RizzyConfig> configBuilder)
    {
        _builder = builder;

        AddHtmxAntiForgery();

        _builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        _builder.Services.AddHttpContextAccessor();
        _builder.Services.Configure<RizzyConfig>(configBuilder);

        // Make sure all HtmxConfig instances get properly configured
        _builder.Services.AddSingleton<IConfigureOptions<HtmxConfig>, ConfigureHtmxSettings>();
        _builder.Services.AddSingleton<IConfigureNamedOptions<HtmxConfig>, ConfigureHtmxSettings>();

        // Configure a default htmx configuration
        _builder.Services.Configure<HtmxConfig>(config => { });

        // Add url helper
        _builder.Services.AddScoped<IUrlHelper>(provider =>
        {
            var helperFactory = provider.GetRequiredService<IUrlHelperFactory>();
            var actionContextAccessor = provider.GetRequiredService<IActionContextAccessor>().ActionContext;

            if (actionContextAccessor is null)
            {
                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor.HttpContext!;
                var actionContext = new ActionContext(httpContext, httpContext.GetRouteData(), new ActionDescriptor());

                return new UrlHelper(actionContext);
            }

            return helperFactory.GetUrlHelper(actionContextAccessor!);
        });

        _builder.Services.AddSingleton<DataAnnotationsProcessor>();

        // Add additional scoped services
        _builder.Services.AddScoped<RzViewContext>();
        _builder.Services.AddScoped<IRizzyService, RizzyService>();
        _builder.Services.AddScoped<IHtmxSwapService, HtmxSwapService>();
    }

    /// <summary>
    /// Adds a default configuration for use inside an HtmxConfigHeadOutlet
    /// </summary>
    /// <param name="configBuilder"></param>
    /// <returns></returns>
    public RizzyConfigBuilder WithHtmxConfiguration(Action<HtmxConfig> configBuilder)
    {
        _builder.Services.Configure<HtmxConfig>(configBuilder);

        return this;
    }

    /// <summary>
    /// Adds a named configuration for use inside an HtmxConfigHeadOutlet
    /// </summary>
    /// <param name="name"></param>
    /// <param name="configBuilder"></param>
    /// <returns></returns>
    public RizzyConfigBuilder WithHtmxConfiguration(string name, Action<HtmxConfig> configBuilder)
    {
        _builder.Services.Configure<HtmxConfig>(name, configBuilder);

        return this;
    }

    private void AddHtmxAntiForgery()
    {
        var provider = _builder.Services.BuildServiceProvider();
        var antiforgeryOptions = provider.GetRequiredService<IOptions<AntiforgeryOptions>>();

        _builder.Services.Configure<HtmxAntiforgeryOptions>(opt =>
        {
            // Default to true, can be configured again after adding htmx if necessary
            opt.FormFieldName = antiforgeryOptions.Value.FormFieldName;
            opt.HeaderName = antiforgeryOptions.Value.HeaderName;
            opt.CookieName = _defaultCookieName;
        });
    }
}