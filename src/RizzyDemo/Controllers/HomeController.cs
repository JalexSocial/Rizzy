using Microsoft.AspNetCore.Mvc;
using RizzyDemo.Models;
using System.Diagnostics;
using Rizzy.Mvc;

namespace RizzyDemo.Controllers;

public class HomeController : RzController
{
	private readonly ILogger<HomeController> _logger;

	public HomeController(ILogger<HomeController> logger)
	{
		_logger = logger;
	}

	public IActionResult Index()
    {
        var data = ViewData;

		return View();
	}

	public IActionResult Privacy()
	{
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
