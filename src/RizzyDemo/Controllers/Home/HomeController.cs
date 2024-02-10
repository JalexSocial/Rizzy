using Microsoft.AspNetCore.Mvc;
using Rizzy.Framework.Mvc;
using RizzyDemo.Components.Shared;
using RizzyDemo.Controllers.Home.Models;
using RizzyDemo.Controllers.Home.Views;
using System.Diagnostics;
using Rizzy.Components.Swap.Services;
using Rizzy.Configuration.Htmx.Enum;
using RizzyDemo.Components.Layout;

namespace RizzyDemo.Controllers.Home;

public class HomeController : RzController
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHtmxSwapService _swapService;

    public HomeController(ILogger<HomeController> logger, IHtmxSwapService swapService)
    {
        _logger = logger;
        _swapService = swapService;
    }

    public IResult Index()
    {
	    _swapService.AddSwappableComponent<NavMenu>("sidebar", null, SwapStyle.InnerHTML);
		_swapService.AddSwappableContent("alert", "<div class=\"alert alert-primary\" role=\"alert\">This content was swapped in from swap service!</div>", SwapStyle.InnerHTML);
        _swapService.AddRawContent("<!--test comment-->");

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
	    _swapService.AddSwappableComponent<NavMenu>("sidebar", null, SwapStyle.InnerHTML);

		return View<Counter>(); 
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IResult Count([FromServices] HtmxCounter.HtmxCounterState state)
    {
	    state.Value++;

	    return View<HtmxCounter>(new { State = state });
    }

    public IResult Weather()
    {
	    _swapService.AddSwappableComponent<NavMenu>("sidebar", null, SwapStyle.InnerHTML);

	    return View<Weather>();
    }

    public IResult Time() => View<Time>();

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
