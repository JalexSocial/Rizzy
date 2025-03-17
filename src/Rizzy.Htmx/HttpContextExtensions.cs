using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;

namespace Rizzy.Htmx;

public static class HttpContextExtensions
{
    /// <summary>
    /// Extension method to check if a request is an Htmx request
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static bool IsHtmx(this HttpRequest request)
    {
        return request.Headers.ContainsKey(HtmxRequestHeaderNames.HtmxRequest);
    }

    /// <summary>
    /// Extension method for HttpRequest that creates (or returns a cached) HtmxRequest.
    /// </summary>
    /// <param name="request">The current HttpRequest.</param>
    /// <returns>An instance of HtmxResponse.</returns>
    public static HtmxRequest Htmx(this HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Check if the HtmxRequest is already created and stored in HttpContext.Items.
        if (request.HttpContext.Items.TryGetValue(Constants.HttpContextKeys.HtmxRequestKey, out var existing) && existing is HtmxRequest htmxRequest)
        {
            return htmxRequest;
        }

        // Create new HtmxResponse and store it in HttpContext.Items.
        htmxRequest = new HtmxRequest(request.HttpContext);
        request.HttpContext.Items[Constants.HttpContextKeys.HtmxRequestKey] = htmxRequest;

        return htmxRequest;
    }

    /// <summary>
    /// Extension method for HttpResponse that creates a new HtmxResponse and invokes the provided action.
    /// </summary>
    /// <param name="response">The current HttpResponse.</param>
    /// <param name="action">An action that receives an HtmxResponse.</param>
    public static void Htmx(this HttpResponse response, Action<HtmxResponse> action)
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentNullException.ThrowIfNull(action);

        // Create a new HtmxResponse.
        var htmxResponse = new HtmxResponse(response.HttpContext);

        // Invoke the action delegate, passing the newly created HtmxResponse.
        action(htmxResponse);
    }

    /// <summary>
    /// Extension method for HttpResponse that creates a new HtmxResponse and returns it.
    /// </summary>
    /// <param name="response">The current HttpResponse.</param>
    /// <returns>A new instance of HtmxResponse.</returns>
    public static HtmxResponse Htmx(this HttpResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        return new HtmxResponse(response.HttpContext);
    }

    /// <summary>
    /// Gets (or creates if missing) the field mapping for the specified <see cref="EditContext"/>.
    /// Uses <see cref="HttpContext.Items"/> as the storage.
    /// </summary>
    /// <param name="context">The current <see cref="HttpContext"/>.</param>
    /// <param name="editContext">The <see cref="EditContext"/> used to identify the mapping.</param>
    /// <returns>A <see cref="Dictionary{FieldIdentifier, RzFormFieldMap}"/> for the given <see cref="EditContext"/>.</returns>
    public static Dictionary<FieldIdentifier, RzFormFieldMap> GetOrAddFieldMapping(
        this HttpContext context, EditContext editContext)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(editContext);

        // Ensure 'ShouldUseFieldIdentifiers' is set as per your existing logic.
        editContext.ShouldUseFieldIdentifiers = true;

        // Get or create the "outer" dictionary that tracks all EditContext => field mappings.
        // If it does not exist, create it and store in HttpContext.Items.
        if (!context.Items.TryGetValue(Constants.HttpContextKeys.FormFieldMappingsKey, out var existing)
            || existing is not Dictionary<EditContext, Dictionary<FieldIdentifier, RzFormFieldMap>> formMappings)
        {
            formMappings = new Dictionary<EditContext, Dictionary<FieldIdentifier, RzFormFieldMap>>();
            context.Items[Constants.HttpContextKeys.FormFieldMappingsKey] = formMappings;
        }

        // Now get or create the specific dictionary for the given EditContext.
        if (!formMappings.TryGetValue(editContext, out var fieldMap))
        {
            fieldMap = new Dictionary<FieldIdentifier, RzFormFieldMap>();
            formMappings[editContext] = fieldMap;
        }

        return fieldMap;
    }

    /// <summary>
    /// Removes the field mapping entry for the given <see cref="EditContext"/> if it exists.
    /// Uses <see cref="HttpContext.Items"/> as the storage.
    /// </summary>
    /// <param name="context">The current <see cref="HttpContext"/>.</param>
    /// <param name="editContext">The <see cref="EditContext"/> used to identify the mapping.</param>
    public static void RemoveFieldMapping(this HttpContext context, EditContext editContext)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(editContext);

        // Look for the "outer" dictionary in HttpContext.Items.
        if (context.Items.TryGetValue(Constants.HttpContextKeys.FormFieldMappingsKey, out var existing)
            && existing is Dictionary<EditContext, Dictionary<FieldIdentifier, RzFormFieldMap>> formMappings)
        {
            // If found, remove the specific dictionary entry for this EditContext.
            formMappings.Remove(editContext);
        }
    }

}