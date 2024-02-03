using Rizzy;
using Rizzy.Components.Layout;
using Rizzy.Configuration.Htmx;
using RizzyDemo.Components.Layout;

var builder = WebApplication.CreateBuilder(args);

builder.AddRizzy(config =>
	{
		config.RootComponent = typeof(HtmxApp<AppLayout>);
        config.DefaultLayout = typeof(HtmxLayout<MainLayout>);
    })
	.WithHtmxConfiguration(config =>
	{
		config.SelfRequestsOnly = true;
	})
	.WithHtmxConfiguration("articles", config =>
	{
		config.SelfRequestsOnly = true;
		config.GlobalViewTransitions = true;
	});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorComponents();

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
app.UseRizzy();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
