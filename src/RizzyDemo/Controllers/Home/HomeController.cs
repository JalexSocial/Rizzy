using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Rizzy.Framework.Mvc;
using RizzyDemo.Components.Shared;
using RizzyDemo.Controllers.Home.Models;
using RizzyDemo.Controllers.Home.Views;

namespace RizzyDemo.Controllers.Home;

public class HomeController : RzController
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IResult Index() => View<HomeIndex>();

    public IResult Privacy() => View<Privacy>();

    public IResult Information() => View<Information>();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IResult Error()
    {
        return View<Error>(new { 
            Model = new ErrorViewModel {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            }
        });
}

}
