using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Components.Forms;
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

    public IResult Information()
    {
        ViewContext.TryAddFormContext("myForm", CurrentActionUrl, new Person());
        
		return View<Information>();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IResult Information([FromForm] Person person)
    {
        ViewContext.TryAddFormContext("myForm", CurrentActionUrl, person);

	    return View<Information>();
    }

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
