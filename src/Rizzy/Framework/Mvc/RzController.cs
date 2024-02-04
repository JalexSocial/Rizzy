using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Rizzy.Components;
using Rizzy.Components.Content;
using Rizzy.Extensions;

namespace Rizzy.Framework.Mvc;
public class RzController : Controller
{
	private EditContext? _editContext;

	/// <summary>
	/// Gets the EditContext
	/// </summary>
	public EditContext EditContext => _editContext;

	public IResult View<TComponent>(object? data = null) where TComponent : IComponent =>
        View<TComponent>(data.ToDictionary());

    public IResult View<TComponent>(Dictionary<string, object?> data) where TComponent : IComponent
    {
        var parameters = new Dictionary<string, object?>();

        RzViewContext context = new RzViewContext (this.HttpContext, 
            this.RouteData, 
            this.ControllerContext.ActionDescriptor,
            this.ModelState,
            this.EditContext);

        context.ComponentParameters = data;

        parameters.Add("ComponentType", typeof(TComponent));
        parameters.Add("ComponentParameters", data);
        parameters.Add("ViewContext", context);

        return new RazorComponentResult<RzPage>(parameters)
        {
            PreventStreamingRendering = false
        };
    }

    public IResult PartialView<TComponent>(object? data = null) where TComponent : IComponent =>
	    PartialView<TComponent>(data.ToDictionary());

    /// <summary>
    /// Renders a Razor component without a layout
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public IResult PartialView<TComponent>(Dictionary<string, object?> data) where TComponent : IComponent
    {
	    var parameters = new Dictionary<string, object?>();

	    RzViewContext context = new RzViewContext(this.HttpContext,
		    this.RouteData,
		    this.ControllerContext.ActionDescriptor,
		    this.ModelState,
		    this.EditContext);

	    context.ComponentParameters = data;

        parameters.Add("ComponentType", typeof(TComponent));
	    parameters.Add("ComponentParameters", data);
	    parameters.Add("ViewContext", context);

	    return new RazorComponentResult<RzPartial>(parameters)
	    {
		    PreventStreamingRendering = false
	    };
    }

    public EditContext CreateEditContext<TModel>(TModel model, bool useDataAnnotations = true) where TModel : class
    {
	    _editContext = new EditContext(model);

        // By default use data annotations as the validator
	    if (useDataAnnotations)
		{
			_editContext.EnableDataAnnotationsValidation(this.HttpContext.RequestServices);
			_editContext.Validate();
		}

		return _editContext;
    }
}
