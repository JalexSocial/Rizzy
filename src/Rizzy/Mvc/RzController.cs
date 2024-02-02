using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rizzy.Components;
using Rizzy.Extensions;

namespace Rizzy.Mvc;
public class RzController : Controller
{
    public IResult View<TComponent>(object? data) where TComponent : IComponent
    {
        var parameters = new Dictionary<string, object?>();

        RzViewContext context = new RzViewContext (this.HttpContext, 
            this.RouteData, 
            this.ControllerContext.ActionDescriptor, 
            this.ModelState);

        parameters.Add("ComponentType", typeof(TComponent));
        parameters.Add("ComponentParameters", data.ToDictionary());
        parameters.Add("ViewContext", context);

        return new RazorComponentResult<RzPage>(parameters)
        {
            PreventStreamingRendering = false
        };
    }
}
