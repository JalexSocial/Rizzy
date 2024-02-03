using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Rizzy.Http;

namespace Rizzy.Framework.Mvc;

public class RzViewContext
{
    public RzViewContext(HttpContext httpContext,
        RouteData routeData,
        ActionDescriptor actionDescriptor,
        ModelStateDictionary modelState
        )
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(routeData);
        ArgumentNullException.ThrowIfNull(actionDescriptor);
        ArgumentNullException.ThrowIfNull(modelState);

        HttpContext = httpContext;
        RouteData = routeData;
        ActionDescriptor = actionDescriptor;
        ModelState = modelState;
        Htmx = new HtmxContext(httpContext);
    }

    public HtmxContext Htmx { get; init; }

    /// <summary>
    /// Gets or sets the <see cref="Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor"/> for the selected action.
    /// </summary>
    /// <remarks>
    /// The property setter is provided for unit test purposes only.
    /// </remarks>
    public ActionDescriptor ActionDescriptor { get; set; } 

    /// <summary>
    /// Gets or sets the <see cref="Microsoft.AspNetCore.Http.HttpContext"/> for the current request.
    /// </summary>
    /// <remarks>
    /// The property setter is provided for unit test purposes only.
    /// </remarks>
    public HttpContext HttpContext { get; set; } 

    /// <summary>
    /// Gets the <see cref="ModelStateDictionary"/>.
    /// </summary>
    public ModelStateDictionary ModelState { get; }

    /// <summary>
    /// Gets or sets the <see cref="AspNetCore.Routing.RouteData"/> for the current request.
    /// </summary>
    /// <remarks>
    /// The property setter is provided for unit test purposes only.
    /// </remarks>
    public Microsoft.AspNetCore.Routing.RouteData RouteData { get; set; }

    public Dictionary<string, object?> ComponentParameters { get; set; } = new();

    public EditContext CreateEditContext<TModel>(TModel model) where TModel : class
    {
	    var editContext = new EditContext(model);
	    var messageStore = new ValidationMessageStore(editContext);

	    // Iterate over the ModelState entries
	    foreach (var state in ModelState)
	    {
		    // Key represents the field name in the model
		    var fieldKey = state.Key;
		    var fieldState = state.Value;

		    // Construct a FieldIdentifier for the current field
		    var fieldIdentifier = new FieldIdentifier(model, fieldKey);

		    // Add model state errors to the ValidationMessageStore
		    foreach (var error in fieldState.Errors)
		    {
			    var errorMessage = error.ErrorMessage;
			    // Add the error message for the field to the message store
			    messageStore.Add(fieldIdentifier, errorMessage);
		    }
	    }

	    // Force the EditContext to consider the newly added messages
	    editContext.NotifyValidationStateChanged();

	    return editContext;
    }
}
