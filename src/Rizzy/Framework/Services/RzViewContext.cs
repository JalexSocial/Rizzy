using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rizzy.Components.Form.Models;
using Rizzy.Http;

namespace Rizzy.Framework.Services;

/// <summary>
/// Represents the context for a view within an application, providing access to HTTP contexts, URL helpers, and component configurations.
/// </summary>
public class RzViewContext(IHttpContextAccessor httpContextAccessor, IUrlHelper urlHelper)
{
    private readonly Dictionary<string, RzFormContext> _formContexts = [];

    /// <summary>
    /// Configures the action context for the view context.
    /// </summary>
    /// <param name="actionContext">The action context to configure.</param>
    /// <exception cref="ArgumentNullException">Thrown if the action context is null.</exception>
    internal void ConfigureActionContext(ActionContext actionContext)
    {
        ArgumentNullException.ThrowIfNull(actionContext);
        ActionContext = actionContext;
    }

    /// <summary>
    /// Configures the view component type and parameters.
    /// </summary>
    /// <param name="componentType">The component type.</param>
    /// <param name="componentParameters">The component parameters.</param>
    /// <exception cref="ArgumentNullException">Thrown if component type or parameters are null.</exception>
    internal void ConfigureView(Type componentType,
        Dictionary<string, object?> componentParameters)
    {
        ArgumentNullException.ThrowIfNull(componentType);
        ArgumentNullException.ThrowIfNull(componentParameters);

        ComponentType = componentType;

        // Merge component parameters
        foreach (var key in componentParameters.Keys)
        {
            ComponentParameters[key] = componentParameters[key];
        }
    }

    /// <summary>
    /// Sets the current page title
    /// </summary>
    public string PageTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets the Htmx context for the current request.
    /// </summary>
    public HtmxContext Htmx => new(HttpContext);

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

    /// <summary>
    /// This is a full list of all the parameters that are set on the component view
    /// </summary>
    public Dictionary<string, object?> ComponentParameters { get; private set; } = new();

    public ActionContext ActionContext { get; private set; } = default!;

    /// <summary>
    /// Attempts to add a form context with the specified name and model.
    /// </summary>
    /// <param name="id">html id for the form</param>
    /// <param name="formName">The name of the form.</param>
    /// <param name="formAction"></param>
    /// <param name="model">The model associated with the form.</param>
    /// <param name="useDataAnnotations">Determines whether to use data annotations for validation.</param>
    /// <returns>True if the form context was added successfully; otherwise, false.</returns>
    public RzFormContext AddFormContext(string id, string formName, string formAction, object model,
        bool useDataAnnotations = true)
    {
        var formContext = string.IsNullOrEmpty(id) ?
            new RzFormContext(formName, formAction, model) :
            new RzFormContext(id, formName, formAction, model);

        // By default use data annotations as the validator
        if (useDataAnnotations)
        {
            formContext.EditContext.EnableDataAnnotationsValidation(this.HttpContext.RequestServices);
            //formContext.EditContext.Validate();
        }

        _formContexts[formName] = formContext;

        return formContext;
    }

    /// <summary>
    /// Attempts to add a form context with the specified name and model.
    /// </summary>
    /// <param name="formName"></param>
    /// <param name="model"></param>
    /// <param name="useDataAnnotations"></param>
    /// <returns></returns>
    public RzFormContext AddFormContext(string formName, object model, bool useDataAnnotations = true) =>
        AddFormContext(string.Empty, formName, string.Empty, model, useDataAnnotations);

    /// <summary>
    /// Attempts to add a form context with the specified name and model.
    /// </summary>
    /// <param name="formName"></param>
    /// <param name="formAction"></param>
    /// <param name="model"></param>
    /// <param name="useDataAnnotations"></param>
    /// <returns></returns>
    public RzFormContext AddFormContext(string formName, string formAction, object model, bool useDataAnnotations = true) =>
        AddFormContext(string.Empty, formName, formAction, model, useDataAnnotations);

    /// <summary>
    /// Attempts to get a form context by name.
    /// </summary>
    /// <param name="formName">The name of the form.</param>
    /// <param name="context">The form context, if found.</param>
    /// <returns>True if the form context was found; otherwise, false.</returns>
    public bool TryGetFormContext(string formName, out RzFormContext context)
    {
        if (!_formContexts.ContainsKey(formName))
        {
            context = null!;
            return false;
        }

        context = _formContexts[formName];

        return true;
    }
}
