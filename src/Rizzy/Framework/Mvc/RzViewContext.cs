using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing;
using Rizzy.Extensions;
using Rizzy.Http;

namespace Rizzy.Framework.Mvc;

public class RzViewContext(IHttpContextAccessor httpContextAccessor, IUrlHelper urlHelper)
{
	private bool _configured = false;
    private string _formUrl = string.Empty;

	public void ConfigureOnce(Type componentType, 
		Dictionary<string, object?> componentParameters,
		EditContext editContext,
		ActionContext actionContext)
	{
		if (_configured) return;

		ArgumentNullException.ThrowIfNull(componentType);
		ArgumentNullException.ThrowIfNull(componentParameters);
		ArgumentNullException.ThrowIfNull(editContext);

		ComponentType = componentType;
		ComponentParameters = ComponentParameters;
		EditContext = editContext;
		ActionContext = actionContext;

        _configured = true;
	}

    public HtmxContext Htmx => new HtmxContext(HttpContext);

    /// <summary>
    /// Gets or sets the <see cref="Microsoft.AspNetCore.Http.HttpContext"/> for the current request.
    /// </summary>
    /// <remarks>
    /// The property setter is provided for unit test purposes only.
    /// </remarks>
    public HttpContext HttpContext => httpContextAccessor.HttpContext!;

    /// <summary>
    /// Provides access to the MVC UrlHelper which contains methods to build URLs for ASP.NET MVC within an application.
    /// </summary>
    public IUrlHelper Url => urlHelper;

	/// <summary>
	/// Gets or sets the AspNetCore.Routing.RouteData for the current request.
	/// </summary>
	/// <remarks>
	/// The property setter is provided for unit test purposes only.
	/// </remarks>
	public RouteData RouteData => HttpContext.GetRouteData();

	public Type ComponentType { get; private set; } = default!;

    public Dictionary<string, object?> ComponentParameters { get; private set; } = new();

    /// <summary>
    /// Gets the EditContext
    /// </summary>
    public EditContext EditContext { get; private set; } = new(new object());

    public ActionContext ActionContext { get; private set; } = default!;

	/// <summary>
	/// Retrieves the current data strongly-typed data model for EditContext. Note that EditContext must be set with a strongly-typed
	/// model for the form model to be accessible.  This is essentially a shortcut for EditContext.Model which does the typecasting for
	/// you.
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	/// <returns></returns>
	/// <exception cref="NullReferenceException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
    public TModel FormModel<TModel>()
    {
	    if (EditContext?.Model is null)
		    throw new NullReferenceException("EditContext was not configured properly on this context");

	    if (EditContext.Model is not TModel)
		    throw new InvalidOperationException($"FormModel is not of type '{typeof(TModel).Name}'. EditContext may have not been configured properly.");

		return (TModel)EditContext.Model;
	}

	/// <summary>
	/// Returns the current action method url as a possible Form callback url but may be overridden manually in any form handler method
	/// This value can be used inside of form Razor Component views
	/// </summary>
    public string FormUrl
    {
        get => _formUrl ?? HttpContext.Request.GetDisplayUrl();
        set => _formUrl = value;
    }
}
