using Microsoft.AspNetCore.Mvc;
using Rizzy.Framework.Mvc;
using RizzyDemo.Components.Shared;
using RizzyDemo.Controllers.Home.Models;
using RizzyDemo.Controllers.Home.Views;
using System.Diagnostics;
using Rizzy.Components.Swap.Services;

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
        _swapService.AddRawContent("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
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
