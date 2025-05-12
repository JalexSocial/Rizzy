using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Rizzy.Framework.Mvc;
using Rizzy;
using Rizzy.Components;
using RizzyBoilerplate.Controllers.Home.Views;
using RizzyBoilerplate.Models;

namespace RizzyBoilerplate.Controllers.Home;

public class HomeController(ILogger<HomeController> logger) : RzController
{
    private readonly ILogger<HomeController> _logger = logger;

    public IResult Index()
    {
        return View<HomeIndex>();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IResult Error()
    {
        return Results.Json(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
