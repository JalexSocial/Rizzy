using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Rizzy;
using Rizzy.Attributes;
using Rizzy.Components;
using Rizzy.Framework.Mvc;
using RizzyDemo.Components.Layout;
using RizzyDemo.Components.Shared;
using RizzyDemo.Controllers.Home.Models;
using RizzyDemo.Controllers.Home.Views;
using System.Diagnostics;
using System.Text;

namespace RizzyDemo.Controllers.Home;

public class HomeController : RzController
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHtmxSwapService _swapService;
    private readonly PostRepo _repo;

    public HomeController(ILogger<HomeController> logger, IHtmxSwapService swapService, PostRepo repo)
    {
        _logger = logger;
        _swapService = swapService;
        _repo = repo;
    }

    public IResult Index()
    {
        _swapService.AddSwappableComponent<NavMenu>("sidebar", null, SwapStyle.innerHTML);
        _swapService.AddRawContent("<!--test comment-->");

        ViewContext.PageTitle = "Home Page";

        return View<HomeIndex>();
    }

    public IResult Privacy() => View<Privacy>();

    public IResult Information()
    {
        ViewContext.AddFormContext("myForm", CurrentActionUrl, new Person());

        return View<Information>();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IResult Information([FromForm] Person person)
    {
        var ctx = ViewContext.AddFormContext("myForm", CurrentActionUrl, person);
        ctx.EditContext.Validate();

        return View<Information>();
    }

    public IResult Counter()
    {
        _swapService.AddSwappableComponent<NavMenu>("sidebar", null, SwapStyle.innerHTML);

        return View<Counter>();
    }

    [HtmxRequest]
    [HttpPost, ValidateAntiForgeryToken]
    public IResult Count([FromServices] HtmxCounter.HtmxCounterState state)
    {
        state.Value++;

        return View<HtmxCounter>(new { State = state });
    }

    public IResult Weather()
    {
        _swapService.AddSwappableComponent<NavMenu>("sidebar", null, SwapStyle.innerHTML);

        return View<Weather>();
    }

    [HtmxRequest]
    public IResult Time()
    {
        if (DateTime.Now.Second % 10 == 0)
        {
            string[] notifications = new string[]
            {
                "Quantum flux capacitor needs recalibration.",
                "The hyperloop buffer overflowed again.",
                "Time-space continuum bandwidth exceeded.",
                "AI neural net became self-aware and is asking for coffee.",
                "Cryptographic hamster wheel generated an unexpected quantum entanglement.",
                "Infinite loop detected in the reality matrix.",
                "Photon torpedo launcher is jammed with spam emails.",
                "The server farm was invaded by digital cows.",
                "Virtual reality headset detected in an alternate dimension.",
                "Parallel universe data leak detected.",
                "Galactic network sync failed due to a space-time anomaly.",
                "Interdimensional web portal needs new SSL certificate.",
                "Subspace signal processor encountered an existential error.",
                "The cloud is literally raining data.",
                "Zero-gravity database migration required immediately."
            };

            Random rand = new Random();
            string message = notifications[rand.Next(notifications.Length)];

            _swapService.AddSwappableContent("alert", $"<div class=\"alert alert-primary\" role=\"alert\">{message}</div>", SwapStyle.innerHTML);
        }

        return PartialView(HomeFragments.RenderServerTime(DateTime.Now));
    }

    public IResult Blog() => View<BlogArticle>();

    [Microsoft.AspNetCore.Mvc.Route("/home/blog/banner")]
    public IResult BlogBanner()
    {
        return PartialView(BlogArticle.RenderBanner($"This banner was changed on the server - {Guid.NewGuid().ToString()}"));
    }

    public async Task<string> News()
    {
        IServiceProvider serviceProvider = HttpContext.RequestServices;
        ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

        await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);
        //await using var htmlRenderer = new ExperimentalHtmlRenderer(serviceProvider, loggerFactory);

        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var dictionary = new Dictionary<string, object?>
            {
                { "Message", "My current mood is excited." }
            };

            var parameters = ParameterView.FromDictionary(dictionary);
            //var output = await htmlRenderer.RenderComponentAsync<MoodLoader>(parameters);

            var component = htmlRenderer.BeginRenderingComponent<MoodLoader>();
            var output = component.ToHtmlString();

            // Give up to 50ms for component to render and then defer to lazy loading
            await Task.WhenAny(component.QuiescenceTask, Task.Delay(50));

            if (!component.QuiescenceTask.IsCompleted)
            {
                await HttpContext.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(output));
                await HttpContext.Response.BodyWriter.FlushAsync();

                await component.QuiescenceTask;
                output = component.ToHtmlString();

                await HttpContext.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(output));
                await HttpContext.Response.BodyWriter.FlushAsync();
            }
            else
            {
                output = component.ToHtmlString();
            }

            return output;
        });

        return html;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IResult Error()
    {
        return View<Error>(new
        {
            Model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            }
        });
    }

}
