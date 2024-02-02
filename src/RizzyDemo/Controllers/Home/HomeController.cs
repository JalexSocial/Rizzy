using Microsoft.AspNetCore.Mvc;
using RizzyDemo.Models;
using System.Diagnostics;
using Rizzy.Mvc;
using RizzyDemo.Controllers.Home.Views;

namespace RizzyDemo.Controllers.Home;

public class HomeController : RzController
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IResult Index()
    {
        var data = ViewData;

        return View<HomeIndex>(new { });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    /*
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
	*/
}
