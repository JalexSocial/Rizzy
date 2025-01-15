using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rizzy.Components;
using Rizzy.Http;
using System.Collections.Concurrent;

namespace Rizzy;

/// <summary>
/// Represents the context for a view within an application, providing access to HTTP contexts, URL helpers, and component configurations.
/// </summary>
[Obsolete("RzViewContext is no longer needed as all functionality has been moved to HttpContext extension methods")]
public class RzViewContext(IHttpContextAccessor httpContextAccessor)
{
    private readonly ConcurrentDictionary<EditContext, Dictionary<FieldIdentifier, RzFormFieldMap>> _formFieldMappings = new();

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

    /// <summary>
    /// Gets (or creates if missing) the field mapping for the given EditContext.
    /// </summary>
    public Dictionary<FieldIdentifier, RzFormFieldMap> GetOrAddFieldMapping(EditContext editContext)
    {
        editContext.ShouldUseFieldIdentifiers = true;

        return _formFieldMappings.GetOrAdd(editContext, _ => new Dictionary<FieldIdentifier, RzFormFieldMap>());
    }

    /// <summary>
    /// Removes the field mapping entry for a given EditContext if no longer needed.
    /// </summary>
    public void RemoveFieldMapping(EditContext editContext)
    {
        _formFieldMappings.TryRemove(editContext, out _);
    }
}
