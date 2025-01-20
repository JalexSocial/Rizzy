using Microsoft.AspNetCore.Mvc;
using Rizzy;
using Rizzy.Components;
using Rizzy.Htmx;
using Rizzy.Nonce;
using RizzyDemo;
using RizzyDemo.Components.Layout;
using RizzyDemo.Components.Shared;

var builder = WebApplication.CreateBuilder(args);

// This must be added after AddControllers
builder.Services.AddRizzy(config =>
{
	config.RootComponent = typeof(HtmxApp<AppLayout>);
	config.DefaultLayout = typeof(HtmxLayout<MainLayout>);
});

builder.Services.AddHtmx(config =>
{
	config.SelfRequestsOnly = true;
});

// Add an alternate named configuration
builder.Services.Configure<HtmxConfig>("articles", config =>
{
	config.SelfRequestsOnly = true;
	config.GlobalViewTransitions = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorComponents();

builder.Services.AddSingleton<HtmxCounter.HtmxCounterState>();
builder.Services.AddSingleton<PostRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseRouting();

app.UseAuthorization();

app.MapPost("/love-htmx",
    ([FromServices] IRizzyService rizzy) => rizzy.PartialView<LoveHtmx>(new { Message = "I ❤️ ASP.NET Core" }));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
