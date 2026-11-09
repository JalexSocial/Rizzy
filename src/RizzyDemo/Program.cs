using Microsoft.AspNetCore.Mvc;
using Rizzy;
using Rizzy.Htmx;
using RizzyDemo;
using RizzyDemo.Components.Layout;
using RizzyDemo.Components.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRizzy(config =>
{
    config.RootComponent = typeof(HtmxApp<AppLayout>);
    config.DefaultLayout = typeof(HtmxLayout<MainLayout>);
});

// htmx 4 defaults: error responses swap, which the validation example relies on (422).
builder.Services.AddHtmx(config => config.Mode = "same-origin");

// Add services to the container.
// AddControllersWithViews registers the MVC antiforgery filters used by [ValidateAntiForgeryToken].
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<HtmxCounter.HtmxCounterState>();
builder.Services.AddSingleton<PostRepo>();

var app = builder.Build();

// Content Security Policy: only scripts carrying this request's nonce may run. The layout puts
// the nonce on its <script> tags, and rizzy.js lets server-rendered scripts in htmx responses
// run while injected ones stay blocked (see the Examples page).
app.Use(async (context, next) =>
{
    var nonce = context.RequestServices.GetRequiredService<IRizzyNonceProvider>().GetNonce();
    context.Response.Headers.ContentSecurityPolicy =
        $"script-src 'nonce-{nonce}'; object-src 'none'; base-uri 'self'";
    await next();
});

// Server errors on fragment requests are not swapped into the target; the client gets an
// rz:server-error event instead. Registered before UseExceptionHandler so it covers its output.
app.UseHtmxErrorHandling();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Required (after authentication and authorization)
app.UseAntiforgery();
app.UseRizzy();

app.MapPost("/love-htmx",
    ([FromServices] IRizzyService rizzy) => rizzy.PartialView<LoveHtmx>(new { Message = "I ❤️ ASP.NET Core" }));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
